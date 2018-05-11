using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JdCat.Cat.Model.Mapping
{
    public class BusinessMapping : IEntityTypeConfiguration<Business>
    {
        public void Configure(EntityTypeBuilder<Business> builder)
        {
            builder.HasMany(a => a.Products)
                .WithOne(a => a.Business)
                .HasForeignKey(a => a.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.ProductsTypes)
                .WithOne(a => a.Business)
                .HasForeignKey(a => a.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
