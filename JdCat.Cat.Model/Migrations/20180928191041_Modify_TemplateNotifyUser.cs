using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_TemplateNotifyUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateNotifyUser",
                schema: "dbo",
                table: "Business");

            migrationBuilder.AddColumn<string>(
                name: "OpenId",
                schema: "dbo",
                table: "Order",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenId",
                schema: "dbo",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "TemplateNotifyUser",
                schema: "dbo",
                table: "Business",
                nullable: true);
        }
    }
}
