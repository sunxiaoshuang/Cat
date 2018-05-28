using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Create_OrderNumbers_FormatNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateSequence<int>(
                name: "FormatNumbers",
                schema: "shared");

            migrationBuilder.CreateSequence<int>(
                name: "OrderNumbers",
                schema: "shared");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "ProductFormat",
                nullable: true,
                defaultValueSql: "NEXT VALUE FOR shared.FormatNumbers",
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "FormatNumbers",
                schema: "shared");

            migrationBuilder.DropSequence(
                name: "OrderNumbers",
                schema: "shared");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "ProductFormat",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "NEXT VALUE FOR shared.FormatNumbers");
        }
    }
}
