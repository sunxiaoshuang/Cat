using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JdCat.Cat.Model.Mapping
{
    public class TangOrderMapping : IEntityTypeConfiguration<TangOrderProduct>
    {
        public void Configure(EntityTypeBuilder<TangOrderProduct> builder)
        {
            builder.HasOne(a => a.TangOrder)
                .WithMany(a => a.TangOrderProducts)
                .HasForeignKey(a => a.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Format)
                .WithMany()
                .HasForeignKey(a => a.FormatId)
                .OnDelete(DeleteBehavior.ClientSetNull);

        }
    }
}
