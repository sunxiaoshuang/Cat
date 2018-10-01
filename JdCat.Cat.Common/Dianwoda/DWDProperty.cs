using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace JdCat.Cat.Common.Dianwoda
{
    public abstract class DWDProperty<T> where T : class
    {
        /// <summary>
        /// 类型属性集合
        /// </summary>
        protected static List<PropertyInfo> PropertyList;
        static DWDProperty()
        {
            var type = typeof(T);
            PropertyList = type.GetProperties().OrderBy(a => a.Name).ToList();
        }
        private string _sign;
        private string _params;
        public void Generate()
        {
            var sb_sign = new StringBuilder();
            var sb_params = new StringBuilder();
            foreach (var property in PropertyList)
            {
                var value = property.GetValue(this);
                //if (value == null) continue;
                sb_sign.Append(property.Name);
                sb_params.Append(property.Name);
                sb_sign.Append(value);
                sb_params.Append($"={value}&");
            }
            sb_params = sb_params.Remove(sb_params.Length - 1, 1);

            _sign = sb_sign.ToString();
            _params = sb_params.ToString();
        }

        public string Sign()
        {
            return _sign;
        }
        public string Params()
        {
            return _params;
        }
    }
}
