using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_BusinessAndOrder_Add_CityCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                schema: "dbo",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                schema: "dbo",
                table: "Business",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityCode",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CityCode",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "CityName",
                schema: "dbo",
                table: "Business");
        }
    }
}
