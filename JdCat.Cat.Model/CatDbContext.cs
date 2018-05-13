using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Mapping;
using Microsoft.EntityFrameworkCore;

namespace JdCat.Cat.Model
{
    public class CatDbContext : DbContext
    {
        public CatDbContext(DbContextOptions<CatDbContext> options) : base(options)
        {

        }

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            optionsBuilder.UseSqlServer("Server=.;Database=jdcat;uid=sa;pwd=sa;");
        //        }

        public DbSet<Business> Businesses { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductFormat> ProductFormats { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<SettingProductAttribute> SettingProductAttributes { get; set; }

//        public DbSet<TestModel> TestModels { get; set; }
        /// <summary>
        /// 添加FluentAPI配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes().Where(q => q.GetInterface(typeof(IEntityTypeConfiguration<>).FullName) != null);
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }
        }
    }
}
