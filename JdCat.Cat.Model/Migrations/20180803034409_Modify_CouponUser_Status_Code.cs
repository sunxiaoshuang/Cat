using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_CouponUser_Status_Code : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "SaleCouponNumbers",
                schema: "shared");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "SaleCouponUser",
                nullable: true,
                defaultValueSql: "dbo.fn_right_padding(floor(rand()*10000000), 6) + cast(NEXT VALUE FOR shared.SaleCouponNumbers as varchar(max)) + dbo.fn_right_padding(floor(rand()*100000), 4)");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "dbo",
                table: "SaleCouponUser",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "SaleCouponNumbers",
                schema: "shared");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "dbo",
                table: "SaleCouponUser");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "SaleCouponUser");
        }
    }
}
