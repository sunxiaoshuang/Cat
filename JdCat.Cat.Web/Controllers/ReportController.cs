using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Report;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace JdCat.Cat.Web.Controllers
{
    public class ReportController : BaseController<IBusinessRepository, Business>
    {
        public ReportController(AppData appData, IBusinessRepository service) : base(appData, service)
        {
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var time = DateTime.Now;
            var orders = Service.GetOrderTotal(Business, time.AddDays(-6), time.AddDays(1));
            for (int i = 0; i < 7; i++)
            {
                var cur = time.AddDays(-i);
                var order = orders.FirstOrDefault(a => a.CreateTime == cur.ToString("yyyy-MM-dd"));
                if (order != null) continue;
                orders.Add(new Report_Order { CreateTime = cur.ToString("yyyy-MM-dd"), Price = 0, Quantity = 0 });
            }
            orders = orders.OrderBy(a => a.CreateTime).ToList();

            ViewBag.list = JsonConvert.SerializeObject(orders, AppData.JsonSetting);
            ViewBag.products = JsonConvert.SerializeObject(Service.GetProductTop10(Business, time), AppData.JsonSetting);
            ViewBag.productPrices = JsonConvert.SerializeObject(Service.GetProductPriceTop10(Business, time), AppData.JsonSetting);
            return View();
        }

        /// <summary>
        /// 销售统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SaleStatistics()
        {
            var time = DateTime.Now;
            var orders = Service.GetOrderTotal(Business, time.AddDays(-6), time.AddDays(1));
            for (int i = 0; i < 7; i++)
            {
                var cur = time.AddDays(-i);
                var order = orders.FirstOrDefault(a => a.CreateTime == cur.ToString("yyyy-MM-dd"));
                if (order != null) continue;
                orders.Add(new Report_Order { CreateTime = cur.ToString("yyyy-MM-dd"), Price = 0, Quantity = 0 });
            }
            orders = orders.OrderBy(a => a.CreateTime).ToList();
            ViewBag.list = JsonConvert.SerializeObject(orders, AppData.JsonSetting);
            return View();
        }

        /// <summary>
        /// 营业统计页
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSaleStatistics([FromQuery]DateTime? start, [FromQuery]DateTime? end)
        {
            if (!start.HasValue || !end.HasValue)
            {
                return Json(new string[] { });
            }
            return Json(Service.GetSaleStatistics(Business, start.Value, end.Value));
        }

        /// <summary>
        /// 导出营业统计
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ExportSaleStatistics([FromQuery]DateTime? start, [FromQuery]DateTime? end)
        {
            var name = $"销售统计({start.Value:yyyyMMdd}-{end.Value:yyyyMMdd}).xlsx";
            var list = Service.GetSaleStatistics(Business, start.Value, end.Value);
            var index = 1;
            var totalPrice = 0d;
            var totalQuantity = 0;
            var totalProductOriginalAmount = 0d;
            var totalProductAmount = 0d;
            var totalFreightAmount = 0d;
            var totalPackageAmount = 0d;
            var totalDiscountAmount = 0d;
            var totalActivity = 0d;
            var totalBenefitAmount = 0d;
            var totalActual = 0d;
            list.ForEach(a =>
            {
                a.Index = index++;
                totalPrice += a.Total;
                totalProductOriginalAmount += a.ProductOriginalAmount;
                totalProductAmount += a.ProductAmount;
                totalFreightAmount += a.FreightAmount;
                totalPackageAmount += a.PackageAmount;
                totalQuantity += a.Quantity;
                totalDiscountAmount += a.DiscountAmount;
                totalActivity += a.ActivityAmount;
                totalBenefitAmount += a.BenefitAmount;
                totalActual += a.ActualTotal;
            });
            list.Add(new Report_SaleStatistics { Index = index, Date = "合计", Total = totalPrice, ProductOriginalAmount = totalProductOriginalAmount, ProductAmount = totalProductAmount, FreightAmount = totalFreightAmount, PackageAmount = totalPackageAmount, Quantity = totalQuantity, DiscountAmount = totalDiscountAmount, ActivityAmount = totalActivity, BenefitAmount = totalBenefitAmount, ActualTotal = totalActual });

            return File(list.ToWorksheet(), AppData.XlsxContentType, name);
        }

        /// <summary>
        /// 商品分析页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ProductAnalysis()
        {
            ViewBag.products = JsonConvert.SerializeObject(Service.GetProductTop10(Business, DateTime.Now), AppData.JsonSetting);
            ViewBag.productPrices = JsonConvert.SerializeObject(Service.GetProductPriceTop10(Business, DateTime.Now), AppData.JsonSetting);
            return View();
        }

        #region 营业统计（堂食）
        public IActionResult Tang()
        {
            return View();
        }
        public async Task<IActionResult> GetTangData([FromQuery]DateTime? start, [FromQuery]DateTime? end)
        {
            if (!start.HasValue || !end.HasValue)
            {
                return Json(new string[] { });
            }
            return Json(await Service.GetSaleStatisticsTangAsync(Business, start.Value, end.Value));
        }
        public async Task<IActionResult> ExportTangData([FromQuery]DateTime? start, [FromQuery]DateTime? end)
        {
            var name = $"销售统计[堂食]({start.Value:yyyyMMdd}-{end.Value:yyyyMMdd}).xlsx";
            var list = await Service.GetSaleStatisticsTangAsync(Business, start.Value, end.Value);
            var index = 1;
            var total = new Report_SaleStatisticsTang { Date = "合计"};
            list.ForEach(a =>
            {
                a.Index = index++;
                total.Quantity += a.Quantity;
                total.GoodAmount += a.GoodAmount;
                total.ActualGoodAmount += a.ActualGoodAmount;
                total.MealFee += a.MealFee;
                total.GoodDiscountAmount += a.GoodDiscountAmount;
                total.OrderDiscountAmount += a.OrderDiscountAmount;
                total.PreferentialAmount += a.PreferentialAmount;
                total.Amount += a.Amount;
                total.ActualAmount += a.ActualAmount;
            });
            total.Index = index;
            list.Add(total);

            return File(list.ToWorksheet(), AppData.XlsxContentType, name);
        }
        #endregion

        #region 厨师
        /// <summary>
        /// 厨师统计
        /// </summary>
        /// <returns></returns>
        public IActionResult Cook()
        {
            return View();
        }
        /// <summary>
        /// 获取厨师统计数据
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetCookData([FromQuery]DateTime start, [FromQuery] DateTime end, [FromQuery]int type, [FromServices]IStoreRepository service)
        {
            var result = type == 0 ? await service.GetCooksReportForTakeoutAsync(Business.ID, start, end.AddDays(1)) : await service.GetCooksReportAsync(Business.ID, start, end.AddDays(1));
            return Json(result ?? new List<Report_ProductSale>());
        }
        /// <summary>
        /// 获取单个厨师的产出
        /// </summary>
        /// <param name="id"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetSingleCookData(int id, [FromQuery]DateTime start, [FromQuery]DateTime end, [FromQuery]int type, [FromServices]IStoreRepository service)
        {
            var result = type == 0 ? await service.GetSingleCookReportForTakeoutAsync(id, start, end.AddDays(1)) : await service.GetSingleCookReportAsync(id, start, end.AddDays(1));
            return Json(result ?? new List<Report_ProductSale>());
        }
        /// <summary>
        /// 导出厨师统计数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExportCookData([FromQuery]DateTime start, [FromQuery] DateTime end, [FromQuery]int type, [FromServices]IStoreRepository service)
        {
            var result = type == 0 ?
                await service.GetCooksReportForTakeoutAsync(Business.ID, start, end.AddDays(1))
                : await service.GetCooksReportAsync(Business.ID, start, end.AddDays(1));
            var total = new Report_ProductSale { Name = "合计", Amount = 0, Count = 0 };
            result.ForEach(item => {
                total.Amount += item.Amount;
                total.Count += item.Count;
            });
            var title = $"厨师{(type == 0 ? "[外卖]" : "")}产出统计";

            var detail = type == 0 ? await service.GetCookDetailReportForTakeoutAsync(result.Select(a => a.Id), start, end.AddDays(1)) : await service.GetCookDetailReportAsync(result.Select(a => a.Id), start, end.AddDays(1));

            using (var package = new ExcelPackage())
            {
                var columnCount = 5;
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].Value = title;
                worksheet.Cells["A2"].Value = $"营业日期：从{start:yyyy-MM-dd}到{end:yyyy-MM-dd}";
                worksheet.Cells["A3"].Value = $"导出时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                worksheet.Cells[1, 1, 1, columnCount].Merge = true;
                worksheet.Cells[2, 1, 2, columnCount].Merge = true;
                worksheet.Cells[3, 1, 3, columnCount].Merge = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4"].Value = "序号";
                worksheet.Cells["B4"].Value = "厨师";
                worksheet.Cells["C4"].Value = "商品";
                worksheet.Cells["D4"].Value = "总数";
                worksheet.Cells["E4"].Value = "总销售额";
                worksheet.Cells[4, 1, 4, columnCount].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[4, 1, 4, columnCount].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                int rowIndex = 5, index = 1;
                foreach (var cook in result)
                {
                    var startRow = rowIndex;
                    worksheet.Cells[$"B{rowIndex}"].Value = cook.Name;
                    var detailList = detail.Where(a => a.Id == cook.Id).ToList();
                    foreach (var item in detailList)
                    {
                        worksheet.Cells[$"A{rowIndex}"].Value = index;
                        worksheet.Cells[$"C{rowIndex}"].Value = item.Name;
                        worksheet.Cells[$"D{rowIndex}"].Value = item.Count;
                        worksheet.Cells[$"E{rowIndex}"].Value = item.Amount;
                        index++;
                        rowIndex++;
                    }
                    worksheet.Cells[$"A{rowIndex}"].Value = index;
                    worksheet.Cells[$"C{rowIndex}"].Value = "合计";
                    worksheet.Cells[$"D{rowIndex}"].Value = cook.Count;
                    worksheet.Cells[$"E{rowIndex}"].Value = cook.Amount;

                    worksheet.Cells[rowIndex - detailList.Count, 2, rowIndex, 2].Merge = true;

                    index++;
                    rowIndex++;
                }
                worksheet.Cells[$"A{rowIndex}"].Value = index;
                worksheet.Cells[$"B{rowIndex}"].Value = "合计";
                worksheet.Cells[$"D{rowIndex}"].Value = total.Count;
                worksheet.Cells[$"E{rowIndex}"].Value = total.Amount;
                worksheet.Cells[5, 1, rowIndex, columnCount].AutoFitColumns();
                var dataBorder = worksheet.Cells[4, 1, rowIndex, columnCount].Style.Border;
                dataBorder.Bottom.Style = dataBorder.Top.Style = dataBorder.Left.Style = dataBorder.Right.Style = ExcelBorderStyle.Thin;
                var xls = package.GetAsByteArray();
                return File(xls, AppData.XlsxContentType, $"{title}({start:yyyyMMdd}-{end:yyyyMMdd}).xlsx");

            }


            //result.Add(total);
            //result.ForEach(a => a.Id = num++);
            //var xls = result.ToWorksheet(title);
            //return File(xls, AppData.XlsxContentType, title + ".xlsx");
        }
        #endregion

        #region 档口
        public IActionResult Booth()
        {
            return View();
        }
        /// <summary>
        /// 获取档口统计数据
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetBoothData([FromQuery]DateTime start, [FromQuery] DateTime end, [FromQuery]int type, [FromServices]IStoreRepository service)
        {
            var result = type == 0 ? await service.GetBoothsReportForTakeoutAsync(Business.ID, start, end.AddDays(1)) : await service.GetBoothsReportAsync(Business.ID, start, end.AddDays(1));
            return Json(result ?? new List<Report_ProductSale>());
        }
        /// <summary>
        /// 获取单个档口的产出
        /// </summary>
        /// <param name="id"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetSingleBoothData(int id, [FromQuery]DateTime start, [FromQuery]DateTime end, [FromQuery]int type, [FromServices]IStoreRepository service)
        {
            var result = type == 0 ? await service.GetSingleBoothReportForTakeoutAsync(id, start, end.AddDays(1)) : await service.GetSingleBoothReportAsync(id, start, end.AddDays(1));
            return Json(result ?? new List<Report_ProductSale>());
        }
        /// <summary>
        /// 导出档口统计数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExportBoothData([FromQuery]DateTime start, [FromQuery] DateTime end, [FromQuery]int type, [FromServices]IStoreRepository service)
        {
            var result = type == 0 ?
                await service.GetBoothsReportForTakeoutAsync(Business.ID, start, end.AddDays(1))
                : await service.GetBoothsReportAsync(Business.ID, start, end.AddDays(1));
            var total = new Report_ProductSale { Name = "合计", Amount = 0, Count = 0 };
            result.ForEach(item => {
                total.Amount += item.Amount;
                total.Count += item.Count;
            });

            var title = $"档口{(type == 0 ? "[外卖]" : "")}产出统计";

            var detail = type == 0 ? await service.GetBoothDetailReportForTakeoutAsync(result.Select(a => a.Id), start, end.AddDays(1)) : await service.GetBoothDetailReportAsync(result.Select(a => a.Id), start, end.AddDays(1));

            using (var package = new ExcelPackage())
            {
                var columnCount = 5;
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].Value = title;
                worksheet.Cells["A2"].Value = $"营业日期：从{start:yyyy-MM-dd}到{end:yyyy-MM-dd}";
                worksheet.Cells["A3"].Value = $"导出时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                worksheet.Cells[1, 1, 1, columnCount].Merge = true;
                worksheet.Cells[2, 1, 2, columnCount].Merge = true;
                worksheet.Cells[3, 1, 3, columnCount].Merge = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4"].Value = "序号";
                worksheet.Cells["B4"].Value = "档口";
                worksheet.Cells["C4"].Value = "商品";
                worksheet.Cells["D4"].Value = "总数";
                worksheet.Cells["E4"].Value = "总销售额";
                worksheet.Cells[4, 1, 4, columnCount].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[4, 1, 4, columnCount].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                int rowIndex = 5, index = 1;
                foreach (var booth in result)
                {
                    var startRow = rowIndex;
                    worksheet.Cells[$"B{rowIndex}"].Value = booth.Name;
                    var detailList = detail.Where(a => a.Id == booth.Id).ToList();
                    foreach (var item in detailList)
                    {
                        worksheet.Cells[$"A{rowIndex}"].Value = index;
                        worksheet.Cells[$"C{rowIndex}"].Value = item.Name;
                        worksheet.Cells[$"D{rowIndex}"].Value = item.Count;
                        worksheet.Cells[$"E{rowIndex}"].Value = item.Amount;
                        index++;
                        rowIndex++;
                    }
                    worksheet.Cells[$"A{rowIndex}"].Value = index;
                    worksheet.Cells[$"C{rowIndex}"].Value = "合计";
                    worksheet.Cells[$"D{rowIndex}"].Value = booth.Count;
                    worksheet.Cells[$"E{rowIndex}"].Value = booth.Amount;

                    worksheet.Cells[rowIndex - detailList.Count, 2, rowIndex, 2].Merge = true;

                    index++;
                    rowIndex++;
                }
                worksheet.Cells[$"A{rowIndex}"].Value = index;
                worksheet.Cells[$"B{rowIndex}"].Value = "合计";
                worksheet.Cells[$"D{rowIndex}"].Value = total.Count;
                worksheet.Cells[$"E{rowIndex}"].Value = total.Amount;
                worksheet.Cells[5, 1, rowIndex, columnCount].AutoFitColumns();
                var dataBorder = worksheet.Cells[4, 1, rowIndex, columnCount].Style.Border;
                dataBorder.Bottom.Style = dataBorder.Top.Style = dataBorder.Left.Style = dataBorder.Right.Style = ExcelBorderStyle.Thin;
                var xls = package.GetAsByteArray();
                return File(xls, AppData.XlsxContentType, $"{title}({start:yyyyMMdd}-{end:yyyyMMdd}).xlsx");

            }

            //result.Add(total);
            //result.ForEach(a => a.Id = num++);
            //var title = $"档口{(type == 0 ? "[外卖]" : "")}产出统计（{start:yyyy年MM月dd日}-{end:yyyy年MM月dd日}）";
            //var xls = result.ToWorksheet(title);
            //return File(xls, AppData.XlsxContentType, title + ".xlsx");
        }
        #endregion

        #region 订单产品

        /// <summary>
        /// 食品销售统计
        /// </summary>
        /// <returns></returns>
        public IActionResult Products()
        {
            return View();
        }
        /// <summary>
        /// 查询食品销售统计数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetProductsData([FromQuery]DateTime start, [FromQuery]DateTime end, [FromQuery]int type, [FromServices]IStoreRepository service)
        {
            var result = type == 0 ? await service.GetProductsDataForTakeoutAsync(Business.ID, start, end.AddDays(1)) : await service.GetProductsDataAsync(Business.ID, start, end.AddDays(1));
            return Json(result);
        }
        /// <summary>
        /// 导出食品销售统计数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExportProductsData([FromQuery]DateTime start, [FromQuery]DateTime end, [FromQuery]int type, [FromServices]IStoreRepository service)
        {
            var list = type == 0 ? await service.GetProductsDataForTakeoutAsync(Business.ID, start, end.AddDays(1)) : await service.GetProductsDataAsync(Business.ID, start, end.AddDays(1));
            var title = "商品销售排行统计" + (type == 0 ? "[外卖]" : "");
            var xls = GetProductsWorksheet(list, title, start, end);
            return File(xls, AppData.XlsxContentType, $"{title}({start:yyyyMMdd}-{end:yyyyMMdd}).xlsx");
        }

        /// <summary>
        /// 第三方订单商品统计
        /// </summary>
        /// <returns></returns>
        [HttpGet("/Report/Third/Products")]
        public IActionResult ThirdProducts()
        {
            return View();
        }
        [HttpGet("/Report/Third/GetProducts")]
        public async Task<IActionResult> GetThirdProductsData([FromQuery]DateTime start, [FromQuery]DateTime end, [FromQuery]int source, [FromServices]IThirdOrderRepository service)
        {
            var result = await service.GetProductsDataAsync(Business.ID, source, start, end.AddDays(1));
            return Json(result);
        }
        [HttpGet("/Report/Third/ExportProducts")]
        public async Task<IActionResult> ExportThirdProductsData([FromQuery]DateTime start, [FromQuery]DateTime end, [FromQuery]int source, [FromServices]IThirdOrderRepository service)
        {
            var result = await service.GetProductsDataAsync(Business.ID, source, start, end.AddDays(1));
            var flag = string.Empty;
            if(source == 99)
            {
                flag = "平台订单";
            }
            else if(source == 0)
            {
                flag = "美团";
            }
            else if(source == 1)
            {
                flag = "饿了么";
            }
            var title = $"商品销售排行统计[{flag}]";
            var xls = GetProductsWorksheet(result, title, start, end);
            return File(xls, AppData.XlsxContentType, $"{title}({start:yyyyMMdd}-{end:yyyyMMdd}).xlsx");
        }
        #endregion

        #region 套餐
        /// <summary>
        /// 套餐统计
        /// </summary>
        /// <returns></returns>
        public IActionResult Setmeal()
        {
            return View();
        }
        /// <summary>
        /// 获取套餐产品数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetSetmealData([FromQuery]DateTime start, [FromQuery]DateTime end, [FromQuery]int type, [FromServices]IStoreRepository service)
        {
            return Json(type == 0 ? await service.GetSetmealDataForTakeoutAsync(Business.ID, start, end.AddDays(1)) : await service.GetSetmealDataAsync(Business.ID, start, end.AddDays(1)));
        }
        /// <summary>
        /// 导出套餐产品数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExportSetmealData([FromQuery]DateTime start, [FromQuery]DateTime end, [FromQuery]int type, [FromServices]IStoreRepository service)
        {
            var list = type == 0 ? await service.GetSetmealDataForTakeoutAsync(Business.ID, start, end.AddDays(1)) : await service.GetSetmealDataAsync(Business.ID, start, end.AddDays(1));
            var title = "套餐产品销售排行" + (type == 0 ? "[外卖]" : "");

            using (var package = new ExcelPackage())
            {
                var columnCount = 5;
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].Value = title;
                worksheet.Cells["A2"].Value = $"营业日期：从{start:yyyy-MM-dd}到{end:yyyy-MM-dd}";
                worksheet.Cells["A3"].Value = $"导出时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                worksheet.Cells[1, 1, 1, columnCount].Merge = true;
                worksheet.Cells[2, 1, 2, columnCount].Merge = true;
                worksheet.Cells[3, 1, 3, columnCount].Merge = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4"].Value = "序号";
                worksheet.Cells["B4"].Value = "商品名称";
                worksheet.Cells["C4"].Value = "销售数量";
                worksheet.Cells["D4"].Value = "来源套餐";
                worksheet.Cells["E4"].Value = "套餐数量";
                worksheet.Cells[4, 1, 4, columnCount].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[4, 1, 4, columnCount].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                int rowIndex = 5, index = 1;
                foreach (var item in list)
                {
                    var startRow = rowIndex;
                    worksheet.Cells[$"A{rowIndex}"].Value = index;
                    worksheet.Cells[$"B{rowIndex}"].Value = item.Name;
                    worksheet.Cells[$"C{rowIndex}"].Value = item.Quantity;
                    foreach (var setmeal in item.SetMeals)
                    {
                        worksheet.Cells[$"D{rowIndex}"].Value = setmeal.Item1;
                        worksheet.Cells[$"E{rowIndex}"].Value = setmeal.Item2;
                        rowIndex++;
                    }
                    var mergeRows = item.SetMeals.Count;
                    if (mergeRows > 1)
                    {
                        worksheet.Cells[startRow, 1, rowIndex - 1, 1].Merge = true;
                        worksheet.Cells[startRow, 2, rowIndex - 1, 2].Merge = true;
                        worksheet.Cells[startRow, 3, rowIndex - 1, 3].Merge = true;
                    }

                    index++;
                }
                worksheet.Cells[5, 1, rowIndex, columnCount].AutoFitColumns();
                var dataBorder = worksheet.Cells[4, 1, rowIndex - 1, columnCount].Style.Border;
                dataBorder.Bottom.Style = dataBorder.Top.Style = dataBorder.Left.Style = dataBorder.Right.Style = ExcelBorderStyle.Thin;
                var xls = package.GetAsByteArray();
                return File(xls, AppData.XlsxContentType, $"{title}({start:yyyyMMdd}-{end:yyyy-MM-dd}).xlsx");

            }
        }

        #endregion

        #region 支付备注统计
        public IActionResult Benefit()
        {
            return View();
        }
        public async Task<IActionResult> GetBenefitData([FromQuery]DateTime start, [FromQuery]DateTime end, [FromServices]IStoreRepository service)
        {
            return Json(await service.GetBenefitDataAsync(Business.ID, start, end.AddDays(1)));
        }
        public async Task<IActionResult> ExportBenefitData([FromQuery]DateTime start, [FromQuery]DateTime end, [FromServices]IStoreRepository service)
        {
            var result = await service.GetBenefitDataAsync(Business.ID, start, end.AddDays(1));
            result.Add(new Report_Benefit
            {
                Name = "合计",
                Amount = result.Sum(a => a.Amount),
                OrderAmount = result.Sum(a => a.OrderAmount),
                Quantity = result.Sum(a => a.Quantity)
            });
            var num = 1;
            result.ForEach(a => a.Index = num++);
            var title = $"优惠统计（{start:yyyy年MM月dd日}-{end:yyyy年MM月dd日}）";
            var xls = result.ToWorksheet(title);
            return File(xls, AppData.XlsxContentType, title + ".xlsx");
        }
        public async Task<IActionResult> GetSingleBenetifData([FromQuery]string name, [FromQuery]DateTime start, [FromQuery]DateTime end, [FromServices]IStoreRepository service)
        {
            var result = await service.GetSingleBenetifDataAsync(Business.ID, name, start, end.AddDays(1));
            return Json(result);
        }

        #endregion

        #region 支付方式统计

        public IActionResult Payment()
        {
            return View();
        }
        public async Task<IActionResult> GetPaymentData([FromQuery]DateTime start, [FromQuery]DateTime end, [FromServices]IStoreRepository service)
        {
            return Json(await service.GetPaymentDataAsync(Business.ID, start, end.AddDays(1)));
        }
        public async Task<IActionResult> ExportPaymentData([FromQuery]DateTime start, [FromQuery]DateTime end, [FromServices]IStoreRepository service)
        {
            var list = await service.GetPaymentDataAsync(Business.ID, start, end.AddDays(1));
            list.Add(new Report_Payment
            {
                Name = "合计", Amount = list.Sum(a => a.Amount), Quantity = list.Sum(a => a.Quantity)
            });
            var title = $"支付方式统计（{start:yyyy年MM月dd日}-{end:yyyy年MM月dd日}）";
            using (var package = new ExcelPackage())
            {
                var columnCount = 4;
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].Value = title;
                worksheet.Cells["A2"].Value = $"导出时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                worksheet.Cells[1, 1, 1, columnCount].Merge = true;
                worksheet.Cells[2, 1, 2, columnCount].Merge = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3"].Value = "序号";
                worksheet.Cells["B3"].Value = "支付方式";
                worksheet.Cells["C3"].Value = "订单数";
                worksheet.Cells["D3"].Value = "支付金额";
                worksheet.Cells[3, 1, 3, columnCount].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[3, 1, 3, columnCount].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                int rowIndex = 4, index = 1;
                foreach (var item in list)
                {
                    var startRow = rowIndex;
                    worksheet.Cells[$"A{rowIndex}"].Value = index;
                    worksheet.Cells[$"B{rowIndex}"].Value = item.Name;
                    worksheet.Cells[$"C{rowIndex}"].Value = item.Quantity;
                    worksheet.Cells[$"D{rowIndex}"].Value = item.Amount;

                    index++;
                    rowIndex++;
                }
                worksheet.Cells[4, 1, rowIndex, columnCount].AutoFitColumns();
                var dataBorder = worksheet.Cells[3, 1, rowIndex - 1, columnCount].Style.Border;
                dataBorder.Bottom.Style = dataBorder.Top.Style = dataBorder.Left.Style = dataBorder.Right.Style = ExcelBorderStyle.Thin;
                var xls = package.GetAsByteArray();
                return File(xls, AppData.XlsxContentType, $"{title}({start:yyyyMMdd}-{end:yyyyMMdd}).xlsx");
            }

        }

        public async Task<IActionResult> GetSinglePaymentData([FromQuery]int id, [FromQuery]DateTime start, [FromQuery]DateTime end, [FromServices]IStoreRepository service)
        {
            return Json(await service.GetSinglePaymentDataAsync(id, start, end.AddDays(1)));
        }



        #endregion


        private byte[] GetProductsWorksheet(List<Report_ProductRanking> list, string title, DateTime start, DateTime end)
        {

            var total = new Report_ProductRanking { Name = "合计" };
            list.ForEach(item => {
                total.Quantity += item.Quantity;
                total.Amount += item.Amount;
                total.CancelQuantity += item.CancelQuantity;
                total.CancelSaleAmount += item.CancelSaleAmount;
                total.CancelAmount += item.CancelAmount;
                total.SaleQuantity += item.SaleQuantity;
                total.SaleAmount += item.SaleAmount;
                total.EntertainQuantity += item.EntertainQuantity;
                total.EntertainAmount += item.EntertainAmount;
                total.DiscountAmount += item.DiscountAmount;
                total.DiscountQuantity += item.DiscountQuantity;
                total.DiscountedAmount += item.DiscountedAmount;
                total.ActualQuantity += item.ActualQuantity;
                total.ActualAmount += item.ActualAmount;
            });
            list.Add(total);

            using (var package = new ExcelPackage())
            {
                var columnCount = 11;
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].Value = title;
                worksheet.Cells["A2"].Value = $"营业日期：从{start:yyyy-MM-dd}到{end:yyyy-MM-dd}";
                worksheet.Cells["A3"].Value = $"导出时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                worksheet.Cells[1, 1, 1, columnCount].Merge = true;
                worksheet.Cells[2, 1, 2, columnCount].Merge = true;
                worksheet.Cells[3, 1, 3, columnCount].Merge = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4"].Value = "序号";
                worksheet.Cells["B4"].Value = "商品名称";
                worksheet.Cells["C4"].Value = "下单数量";
                worksheet.Cells["D4"].Value = "下单总额";
                worksheet.Cells["E4"].Value = "取消数量";
                worksheet.Cells["F4"].Value = "取消总额";
                worksheet.Cells["G4"].Value = "销售数量";
                worksheet.Cells["H4"].Value = "销售总额";
                //worksheet.Cells["I4"].Value = "招待数量";
                //worksheet.Cells["J4"].Value = "招待总额";
                worksheet.Cells["I4"].Value = "折扣总额";
                //worksheet.Cells["L4"].Value = "折后总额";
                worksheet.Cells["J4"].Value = "净售数量";
                worksheet.Cells["K4"].Value = "商品净额";
                worksheet.Cells[4, 1, 4, columnCount].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[4, 1, 4, columnCount].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                int rowIndex = 5, index = 1;
                foreach (var item in list)
                {
                    var startRow = rowIndex;
                    worksheet.Cells[$"A{rowIndex}"].Value = index;
                    worksheet.Cells[$"B{rowIndex}"].Value = item.Name;
                    worksheet.Cells[$"C{rowIndex}"].Value = item.Quantity;
                    worksheet.Cells[$"D{rowIndex}"].Value = item.Amount;
                    worksheet.Cells[$"E{rowIndex}"].Value = item.CancelQuantity;
                    worksheet.Cells[$"F{rowIndex}"].Value = item.CancelSaleAmount;
                    worksheet.Cells[$"G{rowIndex}"].Value = item.SaleQuantity;
                    worksheet.Cells[$"H{rowIndex}"].Value = item.SaleAmount;
                    //worksheet.Cells[$"I{rowIndex}"].Value = item.EntertainQuantity;
                    //worksheet.Cells[$"J{rowIndex}"].Value = item.EntertainAmount;
                    worksheet.Cells[$"I{rowIndex}"].Value = item.DiscountAmount;
                    //worksheet.Cells[$"L{rowIndex}"].Value = item.DiscountedAmount;
                    worksheet.Cells[$"J{rowIndex}"].Value = item.ActualQuantity;
                    worksheet.Cells[$"K{rowIndex}"].Value = item.ActualAmount;

                    index++;
                    rowIndex++;
                }
                worksheet.Cells[5, 1, rowIndex, columnCount].AutoFitColumns();
                var dataBorder = worksheet.Cells[4, 1, rowIndex - 1, columnCount].Style.Border;
                dataBorder.Bottom.Style = dataBorder.Top.Style = dataBorder.Left.Style = dataBorder.Right.Style = ExcelBorderStyle.Thin;
                var xls = package.GetAsByteArray();
                return xls;
            }
        }

    }
}
