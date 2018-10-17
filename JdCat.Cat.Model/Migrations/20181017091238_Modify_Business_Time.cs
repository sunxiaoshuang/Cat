using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Business_Time : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessEndTime2",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessEndTime3",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessStartTime2",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessStartTime3",
                schema: "dbo",
                table: "Business",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessEndTime2",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "BusinessEndTime3",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "BusinessStartTime2",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "BusinessStartTime3",
                schema: "dbo",
                table: "Business");
        }
    }
}
