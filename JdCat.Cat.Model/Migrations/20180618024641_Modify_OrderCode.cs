using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_OrderCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrderCode",
                schema: "dbo",
                table: "Order",
                nullable: true,
                defaultValueSql: "CONVERT(varchar(10), GETDATE(), 112) + dbo.fn_right_padding(NEXT VALUE FOR shared.OrderNumbers, 6) + CAST(floor(rand()*100000) as varchar(5))",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "'Cat-' + CONVERT(varchar(10), GETDATE(), 112) + dbo.fn_right_padding(NEXT VALUE FOR shared.OrderNumbers, 10)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrderCode",
                schema: "dbo",
                table: "Order",
                nullable: true,
                defaultValueSql: "'Cat-' + CONVERT(varchar(10), GETDATE(), 112) + dbo.fn_right_padding(NEXT VALUE FOR shared.OrderNumbers, 10)",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "CONVERT(varchar(10), GETDATE(), 112) + dbo.fn_right_padding(NEXT VALUE FOR shared.OrderNumbers, 6) + CAST(floor(rand()*100000) as varchar(5))");
        }
    }
}
