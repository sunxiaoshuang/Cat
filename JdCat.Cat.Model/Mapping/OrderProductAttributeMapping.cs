using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JdCat.Cat.Model.Mapping
{
    public class OrderProductAttributeMapping : IEntityTypeConfiguration<OrderProductAttribute>
    {
        public void Configure(EntityTypeBuilder<OrderProductAttribute> builder)
        {
            builder.HasOne(a => a.Attribute)
                .WithMany(a => a.OrderProductAttributes)
                .HasForeignKey(a => a.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.OrderProduct)
                .WithMany(a => a.Attributes)
                .HasForeignKey(a => a.OrderProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
