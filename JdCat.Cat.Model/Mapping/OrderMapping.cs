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
            builder.HasOne(a => a.SaleCouponUser)
                .WithOne(a => a.Order)
                .HasForeignKey<Order>(a => a.SaleCouponUserId)
                //.HasForeignKey<SaleCouponUser>(a => a.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}

