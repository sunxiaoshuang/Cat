using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_Business_Feyin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeyinApiKey",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeyinMemberCode",
                schema: "dbo",
                table: "Business",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeyinApiKey",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "FeyinMemberCode",
                schema: "dbo",
                table: "Business");
        }
    }
}
