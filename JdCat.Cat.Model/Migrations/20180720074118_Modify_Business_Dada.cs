using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Business_Dada : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DadaAppKey",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "DadaAppSecret",
                schema: "dbo",
                table: "Business");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DadaAppKey",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DadaAppSecret",
                schema: "dbo",
                table: "Business",
                nullable: true);
        }
    }
}
