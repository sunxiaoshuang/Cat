using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Order_Add_DistributionTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                schema: "dbo",
                table: "Order",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DistributionTime",
                schema: "dbo",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectReasion",
                schema: "dbo",
                table: "Order",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DistributionTime",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RejectReasion",
                schema: "dbo",
                table: "Order");
        }
    }
}
