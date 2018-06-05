using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JdCat.Cat.Model.Mapping
{
    public class OrderMapping : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasMany(a => a.Products)
                .WithOne(a => a.Order)
                .HasForeignKey(a => a.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
