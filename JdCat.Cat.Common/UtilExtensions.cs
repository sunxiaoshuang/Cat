using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
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



    }
}
