using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Business_BusinessTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BusinessEndTime",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BusinessStartTime",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinAmount",
                schema: "dbo",
                table: "Business",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessEndTime",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "BusinessStartTime",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "MinAmount",
                schema: "dbo",
                table: "Business");
        }
    }
}
