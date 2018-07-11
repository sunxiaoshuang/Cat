using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JdCat.Cat.Model.Mapping
{
    public class ShoppingCartMapping : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {

            builder.HasOne(a => a.Format)
                .WithMany(a => a.ShoppingCarts)
                .HasForeignKey(a => a.FormatId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
