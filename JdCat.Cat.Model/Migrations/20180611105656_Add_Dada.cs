using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_Dada : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DadaAppKey",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DadaAppSecret",
                schema: "dbo",
                table: "Business",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
