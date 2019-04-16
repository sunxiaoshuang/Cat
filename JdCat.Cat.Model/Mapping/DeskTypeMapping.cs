using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JdCat.Cat.Model.Mapping
{
    public class DeskTypeMapping : IEntityTypeConfiguration<DeskType>
    {
        public void Configure(EntityTypeBuilder<DeskType> builder)
        {
            builder.HasMany(a => a.Desks)
                .WithOne(a => a.DeskType)
                .HasForeignKey(a => a.DeskTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

        }
    }
}
