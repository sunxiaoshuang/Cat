using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_Order_Discount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                schema: "dbo",
                table: "OrderProduct",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OldPrice",
                schema: "dbo",
                table: "OrderProduct",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                schema: "dbo",
                table: "OrderProduct");

            migrationBuilder.DropColumn(
                name: "OldPrice",
                schema: "dbo",
                table: "OrderProduct");
        }
    }
}
