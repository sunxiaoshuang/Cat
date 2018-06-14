using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Business_DadaAppSecret : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DadaAppSecret",
                schema: "dbo",
                table: "Business",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DadaAppSecret",
                schema: "dbo",
                table: "Business",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
