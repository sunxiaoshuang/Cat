using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JdCat.Cat.Model.Data;
using Microsoft.EntityFrameworkCore;

namespace JdCat.Cat.Model
{
    public class SeedData
    {
        private readonly CatDbContext _context;

        public SeedData(CatDbContext context)
        {
            this._context = context;
        }

        public void Seed()
        {
            InitBusiness();
            InitSettingProductAttribute();
            InitOrder();
        }

        private void InitSettingProductAttribute()
        {
            var count = _context.SettingProductAttributes.Count();
            if (count != 0) return;
            var list = new[]{
                new SettingProductAttribute
                {
                    Name = "温度",
                    Level = 1,
                    Sort = 1,
                    Childs = new[]{
                        new SettingProductAttribute{Name = "热", Sort = 1, Level = 2},
                        new SettingProductAttribute{Name = "温热", Sort = 2, Level = 2},
                        new SettingProductAttribute{Name = "常温", Sort = 3, Level = 2},
                        new SettingProductAttribute{Name = "多冰", Sort = 4, Level = 2},
                        new SettingProductAttribute{Name = "少冰", Sort = 5, Level = 2},
                        new SettingProductAttribute{Name = "冰", Sort = 6, Level = 2}
                    }
                },
                new SettingProductAttribute
                {
                    Name = "辣度",
                    Level = 1,
                    Sort = 2,
                    Childs = new[]{
                        new SettingProductAttribute{Name = "微辣", Sort = 1, Level = 2},
                        new SettingProductAttribute{Name = "中辣", Sort = 2, Level = 2},
                        new SettingProductAttribute{Name = "麻辣", Sort = 3, Level = 2},
                        new SettingProductAttribute{Name = "特辣", Sort = 4, Level = 2},
                        new SettingProductAttribute{Name = "不辣", Sort = 5, Level = 2}
                    }
                },
                new SettingProductAttribute
                {
                    Name = "甜度",
                    Level = 1,
                    Sort = 3,
                    Childs = new[]{
                        new SettingProductAttribute{Name = "无糖", Sort = 1, Level = 2},
                        new SettingProductAttribute{Name = "少糖", Sort = 2, Level = 2},
                        new SettingProductAttribute{Name = "半糖", Sort = 3, Level = 2},
                        new SettingProductAttribute{Name = "多糖", Sort = 4, Level = 2}
                    }
                },
                new SettingProductAttribute
                {
                    Name = "加料",
                    Level = 1,
                    Sort = 3,
                    Childs = new[]{
                        new SettingProductAttribute{Name = "红豆", Sort = 1, Level = 2},
                        new SettingProductAttribute{Name = "珍珠", Sort = 2, Level = 2},
                        new SettingProductAttribute{Name = "椰果", Sort = 3, Level = 2},
                        new SettingProductAttribute{Name = "波霸", Sort = 4, Level = 2}
                    }
                }
            };
            foreach (var item in list)
            {
                _context.SettingProductAttributes.Add(item);
            }
            _context.SaveChanges();
        }

        private void InitBusiness()
        {
            var count = _context.Businesses.Count();
            if (count != 0) return;
            _context.Businesses.Add(new Business
            {
                Name = "系统管理员",
                Address = "湖北省武汉市汉阳区龙阳时代",
                Code = "admin",
                Description = "系统管理员",
                Password = "670b14728ad9902aecba32e22fa4f6bd",
                Mobile = "17354300837",
                RegisterDate = DateTime.Now,
                AppId = "wx7fc7dac038048c37",
                Secret = "79b39d625b3921c2f4bcefe3c4f7c732",
                IsAutoReceipt = true
            });
            _context.SaveChanges();
        }

        private void InitOrder()
        {
            var count = _context.Orders.Count();
            if (count != 0) return;
            var business = _context.Businesses
                .Include("Products.Attributes")
                .Include("Products.Formats")
                .Include("Products.Images")
                .OrderBy(a => a.ID).First();
            var user = _context.Users.FirstOrDefault();
            var product = business.Products.ElementAt(0);
            var product2 = business.Products.ElementAt(0);
            if (user == null || product == null || product2 == null) return;
            var products = new List<OrderProduct> {
                new OrderProduct {
                    Description = "微辣",
                    Product = product,
                    Format =  product.Formats.ElementAt(0),
                    Name = product.Name,
                    Price = product.Formats.ElementAt(0).Price * 2,
                    Quantity = 2,
                    Src = product.Images.ElementAt(0).Name + "." + product.Images.ElementAt(0).ExtensionName
                }
            };
            var products2 = new List<OrderProduct> {
                new OrderProduct {
                    Description = "加料，要椰果",
                    Product = product2,
                    Format =  product2.Formats.ElementAt(0),
                    Name = product2.Name,
                    Price = product2.Formats.ElementAt(0).Price * 2,
                    Quantity = 1,
                    Src = product2.Images.ElementAt(0).Name + "." + product2.Images.ElementAt(0).ExtensionName
                }
            };
            var order1 = new Order
            {
                Business = business,
                DeliveryMode = Enum.DeliveryMode.Third,
                Freight = business.Freight,
                Lat = 30.499750289775,
                Lng = 114.429076910019,
                PaymentType = Enum.PaymentType.OnLine,
                Phone = "17354300837",
                Price = 53,
                Products = products,
                ReceiverAddress = "湖北省武汉市汉阳区人信汇",
                ReceiverName = "张三",
                Remark = "不要辣，不要辣",
                Status = Enum.OrderStatus.Payed,
                DistributionTime = DateTime.Now.AddHours(2),
                TablewareQuantity = 2,
                Tips = 0,
                Type = Enum.OrderType.Food,
                User = user
            };
            var order2 = new Order
            {
                Business = business,
                DeliveryMode = Enum.DeliveryMode.Own,
                Freight = business.Freight,
                Lat = 30.499750289775,
                Lng = 114.429076910019,
                PaymentType = Enum.PaymentType.OnLine,
                Phone = "15544875530",
                Price = 99,
                Products = products2,
                ReceiverAddress = "湖北省武汉市汉阳区人信汇",
                ReceiverName = "张三",
                Remark = "不要辣，不要辣",
                Status = Enum.OrderStatus.Distribution,
                DistributionTime = DateTime.Now.AddHours(1).AddMinutes(28).AddSeconds(33),
                TablewareQuantity = 1,
                Tips = 4,
                Type = Enum.OrderType.Medicine,
                User = user
            };
            _context.Orders.Add(order1);
            _context.Orders.Add(order2);
            _context.SaveChanges();
        }

    }
}
