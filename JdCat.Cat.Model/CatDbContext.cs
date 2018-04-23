using System;
using System.Collections.Generic;
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
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BusinessMapping());
            modelBuilder.ApplyConfiguration(new ProductMapping());
        }
    }
}
