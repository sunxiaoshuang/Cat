using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Order_Code : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "ProductFormat",
                nullable: true,
                defaultValueSql: "'F' + CAST(YEAR(GETDATE()) AS varchar) + dbo.fn_right_padding(NEXT VALUE FOR shared.FormatNumbers, 9)",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "'F-' + CAST(YEAR(GETDATE()) AS varchar) + dbo.fn_right_padding(NEXT VALUE FOR shared.FormatNumbers, 9)");

            migrationBuilder.AlterColumn<string>(
                name: "OrderCode",
                schema: "dbo",
                table: "Order",
                nullable: true,
                defaultValueSql: "CONVERT(varchar(10), GETDATE(), 112) + dbo.fn_right_padding(NEXT VALUE FOR shared.OrderNumbers, 6) + dbo.fn_right_padding(CAST(floor(rand()*100000) as varchar(5)), 5)",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "CONVERT(varchar(10), GETDATE(), 112) + dbo.fn_right_padding(NEXT VALUE FOR shared.OrderNumbers, 6) + CAST(floor(rand()*100000) as varchar(5))");

            migrationBuilder.AlterColumn<string>(
                name: "StoreId",
                schema: "dbo",
                table: "Business",
                nullable: true,
                defaultValueSql: "'JD' + dbo.fn_right_padding(NEXT VALUE FOR shared.StoreNumbers, 6)",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "'JD-' + dbo.fn_right_padding(NEXT VALUE FOR shared.StoreNumbers, 6)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "ProductFormat",
                nullable: true,
                defaultValueSql: "'F-' + CAST(YEAR(GETDATE()) AS varchar) + dbo.fn_right_padding(NEXT VALUE FOR shared.FormatNumbers, 9)",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "'F' + CAST(YEAR(GETDATE()) AS varchar) + dbo.fn_right_padding(NEXT VALUE FOR shared.FormatNumbers, 9)");

            migrationBuilder.AlterColumn<string>(
                name: "OrderCode",
                schema: "dbo",
                table: "Order",
                nullable: true,
                defaultValueSql: "CONVERT(varchar(10), GETDATE(), 112) + dbo.fn_right_padding(NEXT VALUE FOR shared.OrderNumbers, 6) + CAST(floor(rand()*100000) as varchar(5))",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "CONVERT(varchar(10), GETDATE(), 112) + dbo.fn_right_padding(NEXT VALUE FOR shared.OrderNumbers, 6) + dbo.fn_right_padding(CAST(floor(rand()*100000) as varchar(5)), 5)");

            migrationBuilder.AlterColumn<string>(
                name: "StoreId",
                schema: "dbo",
                table: "Business",
                nullable: true,
                defaultValueSql: "'JD-' + dbo.fn_right_padding(NEXT VALUE FOR shared.StoreNumbers, 6)",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "'JD' + dbo.fn_right_padding(NEXT VALUE FOR shared.StoreNumbers, 6)");
        }
    }
}
