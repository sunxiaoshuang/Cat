using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Update_Discount_Time : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StartTime3",
                schema: "dbo",
                table: "SaleProductDiscount",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StartTime2",
                schema: "dbo",
                table: "SaleProductDiscount",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StartTime1",
                schema: "dbo",
                table: "SaleProductDiscount",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EndTime3",
                schema: "dbo",
                table: "SaleProductDiscount",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EndTime2",
                schema: "dbo",
                table: "SaleProductDiscount",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EndTime1",
                schema: "dbo",
                table: "SaleProductDiscount",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime3",
                schema: "dbo",
                table: "SaleProductDiscount",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime2",
                schema: "dbo",
                table: "SaleProductDiscount",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime1",
                schema: "dbo",
                table: "SaleProductDiscount",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime3",
                schema: "dbo",
                table: "SaleProductDiscount",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime2",
                schema: "dbo",
                table: "SaleProductDiscount",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime1",
                schema: "dbo",
                table: "SaleProductDiscount",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
