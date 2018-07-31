using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_SaleFullReduce_ModifyTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyTime",
                schema: "dbo",
                table: "SaleFullReduce",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifyTime",
                schema: "dbo",
                table: "SaleFullReduce");
        }
    }
}
