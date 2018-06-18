
using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JdCat.Cat.Model.Mapping
{
    public class OrderProductMapping : IEntityTypeConfiguration<OrderProduct>
    {
        public void Configure(EntityTypeBuilder<OrderProduct> builder)
        {
            builder.HasOne(a => a.Format)
                .WithMany(a => a.OrderProducts)
                .HasForeignKey(a => a.FormatId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(a => a.Product)
                .WithMany(a => a.OrderProducts)
                .HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
