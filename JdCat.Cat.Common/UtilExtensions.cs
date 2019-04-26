using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JdCat.Cat.Common
{
    public static class UtilExtensions
    {
        private static Dictionary<string, PropertyInfo[]> dicProp;
        static UtilExtensions()
        {
            dicProp = new Dictionary<string, PropertyInfo[]>();
        }
        public static List<T> GetList<T>(this DataTable data, bool ignoreCase = true) where T : new()
        {
            List<T> t = new List<T>();
            int columnscount = data.Columns.Count;
            if (ignoreCase)
            {
                for (int i = 0; i < columnscount; i++)
                    data.Columns[i].ColumnName = data.Columns[i].ColumnName.ToUpper();
            }
            try
            {
                var type = typeof(T);
                PropertyInfo[] properties = null;
                if (dicProp.ContainsKey(type.FullName))
                {
                    properties = dicProp[type.FullName];
                }
                else
                {
                    properties = type.GetProperties();
                    dicProp[type.FullName] = properties;
                }

                var rowscount = data.Rows.Count;
                for (int i = 0; i < rowscount; i++)
                {
                    var model = new T();
                    foreach (var p in properties)
                    {
                        var keyName = p.Name;
                        if (ignoreCase)
                            keyName = keyName.ToUpper();
                        for (int j = 0; j < columnscount; j++)
                        {
                            //if (data.Columns[j].ColumnName == keyName && data.Rows[i][j] != null)
                            //{
                            //    if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            //    {
                            //        p.SetValue(model, Convert.ChangeType(data.Rows[i][j], p.PropertyType.GetGenericArguments()[0]), null);
                            //    }
                            //    else
                            //    {
                            //        p.SetValue(model, Convert.ChangeType(data.Rows[i][j], p.PropertyType), null);
                            //    }
                            //    break;
                            //}
                            if (data.Columns[j].ColumnName == keyName)
                            {
                                if (data.Rows[i][j] == DBNull.Value || data.Rows[i][j] == null) break;
                                p.SetValue(model, data.Rows[i][j], null);
                                break;
                            }
                        }
                    }
                    t.Add(model);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return t;
        }

        /// <summary>
        /// 将内容进行UrlEncode编码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToUrlEncoding(this string content)
        {
            return HttpUtility.UrlEncode(content, Encoding.UTF8);
        }

        public static string ToEncodeSpecial(this string url)
        {
            return url.Replace("+", " ");
        }

        /// <summary>
        /// 将时间转化为时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToInt(this DateTime dateTime)
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// 将列表转化为Excel表格的二进制数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="printerHeaders"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static byte[] ToWorksheet<T>(this IEnumerable<T> list, bool printerHeaders = true)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                var range = worksheet.Cells["A1"].LoadFromCollection(list, printerHeaders);
                for (var col = 1; col < range.Columns + 1; col++)
                {
                    worksheet.Column(col).AutoFit();
                }
                return package.GetAsByteArray();
            }
        }
        public static byte[] ToWorksheet<T>(this IEnumerable<T> list, string title, bool printerHeaders = true, IEnumerable<string> headers = null, string auther = null)
        {
            var rowLen = list.Count() + 2;
            if (printerHeaders) rowLen++;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                var range = worksheet.Cells["A3"].LoadFromCollection(list, printerHeaders);
                var columnsLen = range.Columns;
                worksheet.Cells[3, 1, rowLen, columnsLen].AutoFitColumns();
                var dataBorder = worksheet.Cells[1, 1, rowLen, columnsLen].Style.Border;
                dataBorder.Bottom.Style = dataBorder.Top.Style = dataBorder.Left.Style = dataBorder.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[1, 1, 1, columnsLen].Merge = true;
                worksheet.Cells[2, 1, 2, columnsLen].Merge = true;
                worksheet.Cells[1, 1, 2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.Font.Size = 16;
                worksheet.Cells[1, 1, 1, columnsLen].AutoFitColumns();
                worksheet.Cells[1, 1].Value = title;
                worksheet.Cells[2, 1].Value = $"导出时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                worksheet.Cells[3, 1, 3, columnsLen].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[3, 1, 3, columnsLen].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                if (!string.IsNullOrEmpty(auther)) worksheet.Cells[2, 1].Value += $"，制作人：${auther}";
                return package.GetAsByteArray();
            }
        }

        private const double EARTH_RADIUS = 6378137;
        /// <summary>
        /// 得到两个坐标之间的距离
        /// </summary>
        /// <param name="point1">item1为纬度，item2为经度</param>
        /// <param name="point2">item1为纬度，item2为经度</param>
        /// <returns></returns>
        public static double GetDistance(this Tuple<double, double> point1, Tuple<double, double> point2)
        {
            var radLat1 = Rad(point1.Item1);
            var radLng1 = Rad(point1.Item2);
            var radLat2 = Rad(point2.Item1);
            var radLng2 = Rad(point2.Item2);
            var a = radLat1 - radLat2;
            var b = radLng1 - radLng2;
            var result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;
            return result;
        }
        /// <summary>
        /// 经纬度转化成弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double Rad(double d)
        {
            return d * Math.PI / 180d;
        }

        /// <summary>
        /// 转化为枚举描述文本
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum value, bool nameInstend = true)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return null;
            }
            FieldInfo field = type.GetField(name);
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute == null && nameInstend == true)
            {
                return name;
            }
            return attribute == null ? null : attribute.Description;
        }



    }
}
