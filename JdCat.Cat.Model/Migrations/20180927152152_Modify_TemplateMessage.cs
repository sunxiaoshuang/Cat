using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_TemplateMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrepayId",
                schema: "dbo",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateNotifyId",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateNotifyUser",
                schema: "dbo",
                table: "Business",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrepayId",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TemplateNotifyId",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "TemplateNotifyUser",
                schema: "dbo",
                table: "Business");
        }
    }
}
