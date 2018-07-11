using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_ShoppingCart_BusinessId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                schema: "dbo",
                table: "ShoppingCart",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_BusinessId",
                schema: "dbo",
                table: "ShoppingCart",
                column: "BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_Business_BusinessId",
                schema: "dbo",
                table: "ShoppingCart",
                column: "BusinessId",
                principalSchema: "dbo",
                principalTable: "Business",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_Business_BusinessId",
                schema: "dbo",
                table: "ShoppingCart");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCart_BusinessId",
                schema: "dbo",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                schema: "dbo",
                table: "ShoppingCart");
        }
    }
}
