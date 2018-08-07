using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_CouponUser_UseTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                schema: "dbo",
                table: "SaleCouponUser",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UseTime",
                schema: "dbo",
                table: "SaleCouponUser",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCouponUserId",
                schema: "dbo",
                table: "Order",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_SaleCouponUserId",
                schema: "dbo",
                table: "Order",
                column: "SaleCouponUserId",
                unique: true,
                filter: "[SaleCouponUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_SaleCouponUser_SaleCouponUserId",
                schema: "dbo",
                table: "Order",
                column: "SaleCouponUserId",
                principalSchema: "dbo",
                principalTable: "SaleCouponUser",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_SaleCouponUser_SaleCouponUserId",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_SaleCouponUserId",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderId",
                schema: "dbo",
                table: "SaleCouponUser");

            migrationBuilder.DropColumn(
                name: "UseTime",
                schema: "dbo",
                table: "SaleCouponUser");

            migrationBuilder.DropColumn(
                name: "SaleCouponUserId",
                schema: "dbo",
                table: "Order");
        }
    }
}
