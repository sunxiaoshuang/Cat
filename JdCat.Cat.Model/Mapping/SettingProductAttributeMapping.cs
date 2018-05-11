using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JdCat.Cat.Model.Mapping
{
    public class SettingProductAttributeMapping : IEntityTypeConfiguration<SettingProductAttribute>
    {
        public void Configure(EntityTypeBuilder<SettingProductAttribute> builder)
        {
            
        }
    }
}
