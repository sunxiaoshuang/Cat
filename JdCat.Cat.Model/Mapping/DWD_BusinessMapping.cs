using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JdCat.Cat.Model.Mapping
{
    public class DWD_BusinessMapping : IEntityTypeConfiguration<DWDStore>
    {
        public void Configure(EntityTypeBuilder<DWDStore> builder)
        {
            //builder.HasMany(a => a.DWD_Recharges)
            //    .WithOne(a => a.DWD_Business)
            //    .HasForeignKey(a => a.DWD_BusinessId)
            //    .OnDelete(DeleteBehavior.Cascade);
            //builder.HasOne(a => a.Business)
            //    .WithOne(a => a.DWDStore)
            //    .HasForeignKey<DWDStore>(a => a.BusinessId);

        }
    }
}
