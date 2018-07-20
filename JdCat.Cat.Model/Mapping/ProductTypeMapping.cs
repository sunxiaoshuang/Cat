using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JdCat.Cat.Model.Mapping
{
    public class ProductTypeMapping : IEntityTypeConfiguration<ProductType>
    {
        public void Configure(EntityTypeBuilder<ProductType> builder)
        {
            builder.HasMany(a => a.Products)
                .WithOne(a => a.ProductType)
                .HasForeignKey(a => a.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
