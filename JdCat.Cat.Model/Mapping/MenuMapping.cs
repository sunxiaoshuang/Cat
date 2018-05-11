using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JdCat.Cat.Model.Mapping
{
    public class MenuMapping : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.HasOne(a => a.Parent)
                .WithMany(a => a.Menus)
                .HasForeignKey(a => a.ParentId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(a => a.Roles)
                .
        }
    }
}
