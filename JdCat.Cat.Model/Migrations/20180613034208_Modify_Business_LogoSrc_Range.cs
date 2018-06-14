using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Business_LogoSrc_Range : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoSrc",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Range",
                schema: "dbo",
                table: "Business",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoSrc",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "Range",
                schema: "dbo",
                table: "Business");
        }
    }
}
