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

        }
    }
}
