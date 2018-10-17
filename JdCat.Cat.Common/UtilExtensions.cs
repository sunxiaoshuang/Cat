using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
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
                if(dicProp.ContainsKey(type.FullName))
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
                            if (data.Columns[j].ColumnName == keyName && data.Rows[i][j] != null)
                            {
                                string pval = data.Rows[i][j].ToString();
                                if (!string.IsNullOrEmpty(pval))
                                {
                                    try
                                    {
                                        if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                        {
                                            p.SetValue(model, Convert.ChangeType(data.Rows[i][j], p.PropertyType.GetGenericArguments()[0]), null);
                                        }
                                        else
                                        {
                                            p.SetValue(model, Convert.ChangeType(data.Rows[i][j], p.PropertyType), null);
                                        }
                                    }
                                    catch (Exception x)
                                    {
                                        throw x;
                                    }
                                }
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

        public static string ToUrlEncode(this string url)
        {
            return HttpUtility.UrlEncode(url, Encoding.GetEncoding(936));
        }

        public static string ToEncodeSpecial(this string url)
        {
            return url.Replace("+", " ");
        }

    }
}
