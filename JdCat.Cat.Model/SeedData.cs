using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JdCat.Cat.Model.Data;

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
                Secret = "79b39d625b3921c2f4bcefe3c4f7c732"
            });
        }

    }
}
