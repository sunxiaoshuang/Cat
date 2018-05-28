using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Generator_FormatNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "ProductFormat",
                nullable: true,
                defaultValueSql: "'F-' + CAST(YEAR(GETDATE()) AS varchar) + RIGHT('00000000' + CAST(NEXT VALUE FOR shared.FormatNumbers as varchar), 9)",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "NEXT VALUE FOR shared.FormatNumbers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "ProductFormat",
                nullable: true,
                defaultValueSql: "NEXT VALUE FOR shared.FormatNumbers",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "'F-' + CAST(YEAR(GETDATE()) AS varchar) + RIGHT('00000000' + CAST(NEXT VALUE FOR shared.FormatNumbers as varchar), 9)");
        }
    }
}
