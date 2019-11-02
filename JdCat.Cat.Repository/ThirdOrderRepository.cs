using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Model.Report;
using JdCat.Cat.Repository.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace JdCat.Cat.Repository
{
    public class ThirdOrderRepository : BaseRepository<ThirdOrder>, IThirdOrderRepository
    {
        private IDatabase _database;
        public ThirdOrderRepository(CatDbContext context, IConnectionMultiplexer connection) : base(context)
        {
            _database = connection.GetDatabase();
        }

        public async Task<string> GetMTAppKeyAsync(string appId)
        {
            var key = "Jiandanmao:MT:AppKey:" + appId;
            var secret = await _database.StringGetAsync(key);
            if (secret.HasValue) return secret;
            var business = await Context.Businesses.FirstOrDefaultAsync(a => a.MT_AppId == appId);
            if (business == null) return null;
            await _database.StringSetAsync(key, business.MT_AppKey);
            return business.MT_AppKey;
        }

        public async Task<ThirdOrder> GetOrderByCodeAsync(string order_id)
        {
            return await Context.ThirdOrders.Include(a => a.Business).FirstOrDefaultAsync(a => a.OrderId == order_id);
        }

        public async Task<ThirdOrder> MT_SaveAsync(Dictionary<string, string> dic)
        {
            var business = await Context.Businesses.FirstOrDefaultAsync(a => a.MT_Poi_Id == dic["app_poi_code"]);
            if (business == null) return null;
            var orderId = dic["order_id"];
            if (await Context.ThirdOrders.CountAsync(a => a.OrderId == orderId) > 0)
            {
                // 已存在的订单不再新增
                return null;
            }
            // 订单基本信息
            var deliveryTime = Convert.ToInt64(dic["delivery_time"] ?? "0");           // 预计送达时间
            DateTime? time = null;
            if (deliveryTime > 0) time = deliveryTime.ToDateTime();
            var order = new ThirdOrder
            {
                OrderId = orderId,
                OrderIdView = Convert.ToInt64(dic["wm_order_id_view"]),
                PoiCode = dic["app_poi_code"],
                PoiName = dic["wm_poi_name"],
                PoiAddress = dic["wm_poi_address"],
                PoiPhone = dic["wm_poi_phone"],
                RecipientName = dic["recipient_name"],
                RecipientAddress = dic["recipient_address"],
                RecipientPhone = dic["recipient_phone"],
                ShippingFee = Convert.ToDouble(dic["shipping_fee"]),
                Amount = Convert.ToDouble(dic["total"]),
                OriginalAmount = Convert.ToDouble(dic["original_price"]),
                Caution = dic["caution"],
                InvoiceTitle = dic["invoice_title"],
                TaxpayerId = dic["taxpayer_id"],
                Ctime = Convert.ToInt64(dic["ctime"]).ToDateTime(),
                Utime = Convert.ToInt64(dic["utime"]).ToDateTime(),
                DeliveryTime = time,
                Latitude = Convert.ToDouble(dic["latitude"]),
                Longitude = Convert.ToDouble(dic["longitude"]),
                DaySeq = Convert.ToInt32(dic["day_seq"]),
                Status = OrderStatus.Receipted,
                OrderSource = 0,
                BusinessId = business.ID,
                Business = business
            };
            // 订单商品
            order.ThirdOrderProducts = JArray.Parse(dic["detail"]).Select(product => new ThirdOrderProduct
            {
                Code = product["app_food_code"]?.Value<string>(),
                Name = product["food_name"]?.Value<string>(),
                SkuId = product["sku_id"]?.Value<string>(),
                Quantity = product["quantity"].Value<double>(),
                Price = product["price"].Value<double>(),
                BoxNum = product["box_num"].Value<double>(),
                BoxPrice = product["box_price"].Value<double>(),
                Unit = product["unit"]?.Value<string>(),
                Discount = product["food_discount"].Value<double>(),
                Description = product["food_property"]?.Value<string>(),
                Spec = product["spec"]?.Value<string>(),
                CartId = product["cart_id"].Value<int>()
            }).ToList();
            var names = order.ThirdOrderProducts.Select(a => a.Name).ToList();
            var mappings = await Context.ThirdProductMappings.Where(a => a.ThirdSource == 0 && a.BusinessId == order.BusinessId && names.Contains(a.ThirdProductName)).ToListAsync();
            order.ThirdOrderProducts.ForEach(item =>
            {
                var mapping = mappings.FirstOrDefault(a => a.ThirdProductName == item.Name);
                if (mapping == null) return;
                item.ProductId = mapping.ProductId;
            });
            // 包装费
            order.PackageFee = order.ThirdOrderProducts.Sum(a => a.BoxNum * a.BoxPrice);
            // 订单参与活动
            var activities = JArray.Parse(dic["extras"]);
            if (activities.Count > 0)
            {
                order.ThirdOrderActivities = activities.Select(activity => new ThirdOrderActivity
                {
                    ActiveId = activity["act_detail_id"] == null ? 0 : activity["act_detail_id"].Value<int>(),
                    ReduceFee = activity["reduce_fee"].Value<double>(),
                    ThirdCharge = activity["mt_charge"].Value<double>(),
                    PoiCharge = activity["poi_charge"].Value<double>(),
                    Remark = activity["remark"]?.Value<string>(),
                    Type = activity["type"].Value<int>()
                }).ToList();
            }
            await Context.AddAsync(order);
            await Context.SaveChangesAsync();

            try
            {
                // 自动发送美团订单
                if (business.MT_IsDelivery && business.MT_DeliveryMode != LogisticsType.None)
                {
                    await DeliveryOrderAsync(order, business.MT_DeliveryMode);
                }
            }
            catch (Exception ex)
            {
                Log.Error("美团自动发单错误：" + ex.Message);
            }

            return order;
        }

        public async Task<ThirdOrder> MT_FinishAsync(string id)
        {
            var order = await GetOrderByCodeAsync(id);
            if (order == null) return null;
            order.Status = OrderStatus.Achieve;
            await Context.SaveChangesAsync();
            return order;
        }

        public async Task<ThirdOrder> MT_CancelAsync(string id, string reason)
        {
            var order = await GetOrderByCodeAsync(id);
            if (order == null) return null;
            order.Reason = reason;
            order.Status = OrderStatus.Close;
            await Context.SaveChangesAsync();

            try
            {
                if (order.Business.MT_IsDelivery && order.Business.MT_DeliveryMode != LogisticsType.None)
                {
                    await UnDeliveryOrderAsync(order, reason);
                }
            }
            catch (Exception ex)
            {
                Log.Error("美团自动发单错误：" + ex.Message);
            }

            return order;
        }

        public async Task<Business> GetBusinessByMtPoi(string poi)
        {
            return await Context.Businesses.FirstOrDefaultAsync(a => a.MT_Poi_Id == poi);
        }


        public async Task<string> GetElemeAppSecretAsync(long appId)
        {
            var key = "Jiandanmao:Eleme:AppSecret:" + appId;
            var secret = await _database.StringGetAsync(key);
            if (secret.HasValue) return secret;
            var business = await Context.Businesses.FirstOrDefaultAsync(a => a.Eleme_AppId == appId);
            if (business == null) return null;
            await _database.StringSetAsync(key, business.Eleme_AppSecret);
            return business.Eleme_AppSecret;
        }

        public async Task<ThirdOrder> ElemeSaveAsync(JObject message)
        {
            var shopId = message["shopId"].Value<long>();
            var eleme = JObject.Parse(message["message"].Value<string>());
            var orderId = eleme["id"].Value<string>();
            if (await Context.ThirdOrders.CountAsync(a => a.OrderId == orderId) > 0) return null;   // 如果已存在该订单，则业务终止    
            var business = await Context.Businesses.FirstOrDefaultAsync(a => a.Eleme_Poi_Id == shopId);
            if (business == null) return null;

            var phone = eleme["phoneList"]?.FirstOrDefault();
            var geo = eleme["deliveryGeo"].Value<string>().Split(',');

            var order = new ThirdOrder
            {
                OrderId = eleme["id"].Value<string>(),
                PoiCode = eleme["shopId"].ToString(),
                PoiName = eleme["shopName"].Value<string>(),
                RecipientName = eleme["consignee"].Value<string>(),
                RecipientAddress = eleme["address"].Value<string>(),
                RecipientPhone = phone?.Value<string>(),
                ShippingFee = eleme["deliverFee"].Value<double>(),
                PackageFee = eleme["packageFee"].Value<double>(),
                Amount = eleme["totalPrice"].Value<double>(),
                OriginalAmount = eleme["originalPrice"].Value<double>(),
                Caution = eleme["description"].Value<string>(),
                InvoiceTitle = eleme["invoice"].Value<string>(),
                TaxpayerId = eleme["taxpayerId"].Value<string>(),
                Ctime = eleme["createdAt"].Value<DateTime>(),
                DeliveryTime = eleme["deliverTime"].Value<DateTime?>(),
                Longitude = Convert.ToDouble(geo[0]),
                Latitude = Convert.ToDouble(geo[1]),
                DaySeq = eleme["daySn"].Value<int>(),
                Status = OrderStatus.Payed,
                OrderSource = 1,
                BusinessId = business.ID,
                ThirdOrderProducts = new List<ThirdOrderProduct>()
            };
            // 订单商品
            var detail = eleme["groups"];
            var cardIndex = 1;
            foreach (var box in detail)
            {
                foreach (var p in box["items"])
                {
                    var product = new ThirdOrderProduct
                    {
                        Code = p["vfoodId"].ToString(),
                        Name = p["name"].Value<string>(),
                        SkuId = p["skuId"].ToString(),
                        Quantity = p["quantity"].Value<int>(),
                        Price = p["price"].Value<double>(),
                        Spec = string.Join(',', p["newSpecs"].Select(a => a["value"].ToString())),
                        Description = string.Join(',', p["attributes"].Select(a => a["value"].ToString())),
                        //Discount = Math.Round(p["price"].Value<double>() / p["shopPrice"].Value<double>(), 2),
                        Discount = 1,
                        CartId = cardIndex
                    };
                    order.ThirdOrderProducts.Add(product);
                }
                cardIndex++;
            }
            var codes = order.ThirdOrderProducts.Select(a => a.Code).ToList();          // 饿了么使用商品id匹配
            var mappings = await Context.ThirdProductMappings.Where(a => a.ThirdSource == 1 && a.BusinessId == order.BusinessId && codes.Contains(a.ThirdProductId)).ToListAsync();
            order.ThirdOrderProducts.ForEach(item =>
            {
                var mapping = mappings.FirstOrDefault(a => a.ThirdProductId == item.Code);
                if (mapping == null) return;
                item.ProductId = mapping.ProductId;
            });
            // 活动信息
            var activities = eleme["orderActivities"];
            if (activities.HasValues && activities.Count() > 0)
            {
                order.ThirdOrderActivities = new List<ThirdOrderActivity>();
                foreach (var item in activities)
                {
                    var activity = new ThirdOrderActivity
                    {
                        ActiveId = item["metaId"].Value<int>(),
                        ReduceFee = item["amount"].Value<double>(),
                        ThirdCharge = item["elemePart"].Value<double>() + item["familyPart"].Value<double>(),
                        PoiCharge = item["restaurantPart"].Value<double>(),
                        Remark = item["name"].Value<string>()
                    };
                    order.ThirdOrderActivities.Add(activity);
                }
            }
            await Context.AddAsync(order);
            await Context.SaveChangesAsync();

            return order;
        }

        public async Task<ThirdOrder> ElemeReceivedAsync(JObject message)
        {
            var notify = JObject.Parse(message["message"].Value<string>());
            var orderId = notify["orderId"].Value<string>();
            var order = await Context.ThirdOrders.Include(a => a.ThirdOrderProducts).Include(a => a.ThirdOrderActivities).FirstOrDefaultAsync(a => a.OrderId == orderId);
            //if (order == null) throw new Exception($"订单[{orderId}]不存在");
            if (order == null)
            {
                // 如果订单为空，则等待一秒钟再获取
                Thread.Sleep(1000);
                order = await Context.ThirdOrders.Include(a => a.ThirdOrderProducts).Include(a => a.ThirdOrderActivities).FirstOrDefaultAsync(a => a.OrderId == orderId);
                if (order == null) return null;
            }
            order.Status = OrderStatus.Receipted;
            await Context.SaveChangesAsync();
            return order;
        }

        public async Task<ThirdOrder> ElemeFinishAsync(JObject message)
        {
            var notify = JObject.Parse(message["message"].Value<string>());
            var orderId = notify["orderId"].Value<string>();
            var order = await Context.ThirdOrders.FirstOrDefaultAsync(a => a.OrderId == orderId);
            if (order == null) return null;
            order.Status = OrderStatus.Achieve;
            await Context.SaveChangesAsync();
            return order;
        }

        public async Task<ThirdOrder> ElemeCancelAsync(JObject message)
        {
            var notify = JObject.Parse(message["message"].Value<string>());
            var orderId = notify["orderId"].Value<string>();
            var reason = string.Empty;
            if (notify.ContainsKey("reason"))
            {
                reason = notify["reason"].Value<string>();
            }
            var order = await Context.ThirdOrders.FirstOrDefaultAsync(a => a.OrderId == orderId);
            if (order == null) return null;
            var status = order.Status;
            if (status == OrderStatus.Receipted)
            {
                // 如果订单处于接单状态，则需要取消通知
                message["cancel"] = true;
            }
            else
            {
                message["cancel"] = false;
            }
            order.Status = OrderStatus.Cancel;
            order.Reason = reason;
            await Context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> AddOrderNotifyAsync(ThirdOrder order, bool isTimes = false)
        {
            // 如果是补打，则将对象复制一遍，并将预约时间设置为空
            if (isTimes)
            {
                order = JsonConvert.DeserializeObject<ThirdOrder>(JsonConvert.SerializeObject(order, AppData.JsonSetting));
                order.DeliveryTime = null;
            }

            await LoadSetmealAsync(order.ThirdOrderProducts);
            order.Business = null;
            var deliveryTime = order.DeliveryTime;
            var now = DateTime.Now;
            TimeSpan timespan;
            if (deliveryTime != null && deliveryTime > now)
            {
                // 送达时间之后，两小时过期
                timespan = deliveryTime.Value.AddHours(2) - now;
            }
            else
            {
                // 没有送达时间，则当前时间过后两小时过期
                timespan = new TimeSpan(2, 0, 0);
            }
            return await _database.StringSetAsync($"Jiandanmao:Notify:ThirdOrder:{order.BusinessId}:{order.ID}", JsonConvert.SerializeObject(order, AppData.JsonSetting), timespan);
        }

        public async Task DeliveryOrderAsync(ThirdOrder order, LogisticsType logistics)
        {
            if (order.Business == null) order.Business = await Context.Businesses.FirstOrDefaultAsync(a => a.ID == order.BusinessId);
            switch (logistics)
            {
                case LogisticsType.Dada:
                    //return await DadaHandler(order);
                    break;
                case LogisticsType.Dianwoda:
                    //return await DwdHandler(order);
                    break;
                case LogisticsType.Fengniao:
                    break;
                case LogisticsType.Meituan:
                    break;
                case LogisticsType.Self:
                    //return SelfHandler(order);
                    break;
                case LogisticsType.Yichengfeike:
                    await YcfkHandlerAsync(order);
                    break;
                case LogisticsType.Shunfeng:
                    //return await SfHandler(order);
                    break;
                default:
                    break;
            }
            await Context.SaveChangesAsync();
        }

        public async Task UnDeliveryOrderAsync(ThirdOrder order, string reason)
        {
            switch (order.LogisticsType)
            {
                case LogisticsType.Dada:
                    break;
                case LogisticsType.Dianwoda:
                    break;
                case LogisticsType.Fengniao:
                    break;
                case LogisticsType.Meituan:
                    break;
                case LogisticsType.Self:
                    break;
                case LogisticsType.Yichengfeike:
                    await UnYcfkHandlerAsync(order, reason);
                    break;
                case LogisticsType.Shunfeng:
                    break;
                default:
                    break;
            }
            await Context.SaveChangesAsync();
        }


        public async Task<string> GetElemeTokenAsync(string url, string appKey, string appSecret)
        {
            var key = "Jiandanmao:Eleme:Access_Token:" + appKey;
            var token = await _database.StringGetAsync(key);
            var now = DateTime.Now;
            if (token.HasValue)
            {
                var json = JObject.Parse(token);
                var getTime = json["getTime"].Value<DateTime>();
                var expires_in = json["expires_in"].Value<int>();
                if (getTime.AddSeconds(expires_in - 172800) > now)          // 如果还有两天过期，则需要刷新token
                {
                    return json["access_token"].Value<string>();
                }
            }

            // 如果token不存在或者token已过期，再执行下面的逻辑
            return await RefreshElemeTokenAsync(url, appKey, appSecret);
        }

        public async Task<string> RefreshElemeTokenAsync(string url, string key, string secret)
        {
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{key}:{secret}"));
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

                var body = new StringContent("grant_type=client_credentials");
                body.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var res = await client.PostAsync(url, body);
                var content = await res.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                if (json["access_token"] == null)
                {
                    throw new Exception("饿了么获取token异常：" + json["error"].Value<string>());
                }
                var access_token = json["access_token"].Value<string>();
                json["getTime"] = DateTime.Now;
                await _database.StringSetAsync(key, JsonConvert.SerializeObject(json));
                Log.Info($"商户key：{key}，token刷新成功");
                return access_token;
            }

        }

        public async Task<List<ThirdProductMapping>> GetProductMappingsAsync(int businessId, int source)
        {
            return await Context.ThirdProductMappings.Where(a => a.BusinessId == businessId && a.ThirdSource == source).ToListAsync();
        }
        public async Task ClearMappingsAsync(int businessId, int source)
        {
            var list = await Context.ThirdProductMappings.Where(a => a.BusinessId == businessId && a.ThirdSource == source).ToListAsync();
            Context.RemoveRange(list);
            await Context.SaveChangesAsync();
        }
        public async Task SetProductMappingsAsync(IEnumerable<ThirdProductMapping> mappings)
        {
            var source = mappings.FirstOrDefault().ThirdSource;
            var businessId = mappings.FirstOrDefault().BusinessId;
            var names = mappings.Select(a => a.ThirdProductName).ToList();
            var codes = mappings.Select(a => a.ThirdProductId).Where(a => !string.IsNullOrEmpty(a)).ToList();
            var products = await Context.ThirdProductMappings.Where(a => a.BusinessId == businessId && a.ThirdSource == source && (codes.Contains(a.ThirdProductId) || names.Contains(a.ThirdProductName))).ToListAsync();
            Context.RemoveRange(products);
            Context.AddRange(mappings);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateOrderStatusAsync(YcfkCallback ycfk)
        {
            var id = ycfk.OrderId.Split('_')[0];
            var order = await Context.ThirdOrders.Include(a => a.Business).FirstOrDefaultAsync(a => a.OrderId == id);
            
            if (order == null) return;
            switch (ycfk.OrderState)
            {
                case 1:             // 已处理
                case 21:            // 等待分配骑手
                case 22:            // 取餐中
                    order.Status = OrderStatus.DistributorReceipt;
                    break;
                case 23:            // 配送中
                    order.Status = OrderStatus.Distribution;
                    await MT_DeliveryOrderAsync(id, order.Business);
                    break;
                case 100:           // 同意退款
                case 101:           // 拒绝退款
                case 255:           // 已关闭

                    break;
                case 254:           // 已完成
                    order.Status = OrderStatus.Achieve;
                    await MT_FinishOrderAsync(id, order.Business);
                    break;
                default:
                    break;
            }
            await Context.SaveChangesAsync();
        }


        public async Task<List<ThirdOrder>> GetOrdersAsync(int businessId, int source, DateTime start, DateTime end, PagingQuery paging, int dayNum)
        {
            var query = Context.ThirdOrders.Where(a => a.BusinessId == businessId && a.Ctime >= start && a.Ctime < end);
            if (source != 99)
            {
                query = query.Where(a => a.OrderSource == source);
            }
            if (dayNum > 0)
            {
                query = query.Where(a => a.DaySeq == dayNum);
            }
            paging.RecordCount = query.Count();
            return await query.OrderByDescending(a => a.ID).Skip(paging.Skip).Take(paging.PageSize).ToListAsync();
        }
        public async Task<ThirdOrder> GetOrderDetailAsync(int id)
        {
            return await Context.ThirdOrders
                .Include(a => a.ThirdOrderProducts)
                .Include(a => a.ThirdOrderActivities)
                .FirstOrDefaultAsync(a => a.ID == id);
        }


        public async Task<List<Report_ProductRanking>> GetProductsDataAsync(int businessId, int source, DateTime start, DateTime end)
        {
            var query = from order in Context.ThirdOrders
                        where order.BusinessId == businessId && (order.Status & OrderStatus.Valid) > 0 && order.Ctime >= start && order.Ctime < end
                        select order;
            if(source != 99)
            {
                query = query.Where(a => a.OrderSource == source);
            }
            var list = from order in query
                    join product in Context.ThirdOrderProducts on order.ID equals product.ThirdOrderId
                    group product by product.Name into g
                    select new Report_ProductRanking
                    {
                        Name = g.Key,
                        Quantity = g.Sum(a => a.Quantity),
                        //Amount = g.Sum(a => a.Price),
                        ActualAmount = g.Sum(a => a.Price * a.Quantity),
                        DiscountQuantity = 0,
                        DiscountAmount = 0,
                        DiscountedAmount = 0
                    };
            var rows = await list.ToListAsync();
            rows.ForEach(item =>
            {
                item.Amount = item.ActualAmount;
                item.SaleAmount = item.Amount;
                item.ActualQuantity = item.Quantity;
                item.SaleQuantity = item.Quantity;
            });
            rows.Sort((a, b) => (int)(b.ActualAmount - a.ActualAmount));
            return rows;
        }


        /// <summary>
        /// 加载商品列表中，套餐的关联商品，Tag1属性存储
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        private async Task LoadSetmealAsync(IEnumerable<ThirdOrderProduct> products)
        {
            var ids = products.Select(a => a.ProductId);
            var setmeals = await Context.Products
                .Where(a => a.Feature == ProductFeature.SetMeal && ids.Contains(a.ID))
                .Select(a => a.ID)
                .ToListAsync();
            if (setmeals.Count == 0) return;
            var relatives = await Context.ProductRelatives.Where(a => setmeals.Contains(a.SetMealId)).ToListAsync();
            if (relatives.Count == 0) return;
            var childIds = relatives.Select(a => a.ProductId).Distinct();
            var children = await Context.Products.Where(a => childIds.Contains(a.ID)).ToListAsync();
            children.ForEach(a => a.Business = null);
            setmeals.ForEach(a =>
            {
                var setmeal = products.First(b => b.ProductId == a);
                var leafIds = relatives.Where(b => b.SetMealId == a).Select(b => b.ProductId);
                setmeal.Tag1 = children.Where(b => leafIds.Contains(b.ID)).ToList();
            });
        }
        /// <summary>
        /// 一城飞客发单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task YcfkHandlerAsync(ThirdOrder order)
        {
            var helper = YcfkHelper.GetHelper();
            var ycfkOrder = new YcfkOrder
            {
                OrderId = $"{order.OrderId}_{UtilHelper.RandNum()}_{order.OrderSource}",
                Flag = order.OrderSource == 0 ? "美团" : "饿了么",
                ViewOrderId = order.OrderId,
                ShopId = order.Business.StoreId,
                ShopName = order.Business.Name,
                OrderUserName = order.RecipientName,
                OrderUserPhone = order.RecipientPhone,
                OrderUserAddress = order.RecipientAddress,
                OrderRemark = order.Caution,
                BoxFee = Convert.ToDecimal(order.PackageFee),
                Freight = Convert.ToDecimal(order.ShippingFee),
                ActivityMoney = Convert.ToDecimal(order.OriginalAmount - order.Amount),
                ReachTime = order.DeliveryTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                UserGaodeCoordinate = order.Longitude + "|" + order.Latitude,
                DayIndex = order.DaySeq
            };

            ycfkOrder.FoodList = order.ThirdOrderProducts.Select(a => new YcfkFoodItem
            {
                FoodName = a.Name,
                FoodPrice = Convert.ToDecimal(a.Price / a.Quantity),
                FoodCount = Convert.ToInt32(a.Quantity)
            }).ToList();

            var json = await helper.Send(ycfkOrder, order.Business.YcfkKey, order.Business.YcfkSecret);
            try
            {
                var jObj = JObject.Parse(json);
                var code = jObj["StateCode"].Value<int>();
                if (code > 0)
                {
                    order.Error = jObj["StateMsg"].Value<string>();
                    //Log.Debug("一城飞客配送异常：" + json);
                    return;
                }
            }
            catch (Exception e)
            {
                throw new Exception("一城飞客自动发单异常：" + e.Message + $"。返回值：【{json}】");
            }
            //Log.Debug("一城飞客配送成功：" + json);

            order.Status = OrderStatus.DistributorReceipt;
            order.LogisticsType = LogisticsType.Yichengfeike;
            order.Error = "";
            order.Data1 = ycfkOrder.OrderId;
        }
        /// <summary>
        /// 取消一城飞客配送
        /// </summary>
        /// <param name="order"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        private async Task UnYcfkHandlerAsync(ThirdOrder order, string reason)
        {
            var helper = YcfkHelper.GetHelper();
            await helper.Cancel(order.Data1, reason, order.Business.YcfkKey, order.Business.YcfkSecret);
        }

        /// <summary>
        /// 美团订单配送中
        /// </summary>
        /// <returns></returns>
        private async Task MT_DeliveryOrderAsync(string id, Business business)
        {
            var url = "https://waimaiopen.meituan.com/api/v1/order/delivering";
            var mt = new MTInputData(business.MT_AppKey, url);
            mt.SetValue("timestamp", DateTime.Now.ToTimestamp());
            mt.SetValue("app_id", business.MT_AppId);
            mt.SetValue("order_id", id);
            var sig = mt.MakeSign();
            mt.SetValue("sig", sig);
            url = $"{url}?{mt.ToUrl()}";
            await UtilHelper.RequestAsync(url, method: "get");
        }
        /// <summary>
        /// 美团订单已完成
        /// </summary>
        /// <returns></returns>
        private async Task MT_FinishOrderAsync(string id, Business business)
        {
            var url = "https://waimaiopen.meituan.com/api/v1/order/arrived";
            var mt = new MTInputData(business.MT_AppKey, url);
            mt.SetValue("timestamp", DateTime.Now.ToTimestamp());
            mt.SetValue("app_id", business.MT_AppId);
            mt.SetValue("order_id", id);
            var sig = mt.MakeSign();
            mt.SetValue("sig", sig);
            url = $"{url}?{mt.ToUrl()}";
            await UtilHelper.RequestAsync(url, method: "get");
        }

    }
}
