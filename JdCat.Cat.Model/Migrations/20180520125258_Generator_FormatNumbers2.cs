using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Generator_FormatNumbers2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "ProductFormat",
                nullable: true,
                defaultValueSql: "'F-' + CAST(YEAR(GETDATE()) AS varchar) + dbo.fn_right_padding(NEXT VALUE FOR shared.FormatNumbers, 9)",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "'F-' + CAST(YEAR(GETDATE()) AS varchar) + RIGHT('00000000' + CAST(NEXT VALUE FOR shared.FormatNumbers as varchar), 9)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "ProductFormat",
                nullable: true,
                defaultValueSql: "'F-' + CAST(YEAR(GETDATE()) AS varchar) + RIGHT('00000000' + CAST(NEXT VALUE FOR shared.FormatNumbers as varchar), 9)",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "'F-' + CAST(YEAR(GETDATE()) AS varchar) + dbo.fn_right_padding(NEXT VALUE FOR shared.FormatNumbers, 9)");
        }
    }
}
