using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_Order_SaleFullReduceId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_SaleFullReduce_SaleFullReduceID",
                schema: "dbo",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "SaleFullReduceID",
                schema: "dbo",
                table: "Order",
                newName: "SaleFullReduceId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_SaleFullReduceID",
                schema: "dbo",
                table: "Order",
                newName: "IX_Order_SaleFullReduceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_SaleFullReduce_SaleFullReduceId",
                schema: "dbo",
                table: "Order",
                column: "SaleFullReduceId",
                principalSchema: "dbo",
                principalTable: "SaleFullReduce",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_SaleFullReduce_SaleFullReduceId",
                schema: "dbo",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "SaleFullReduceId",
                schema: "dbo",
                table: "Order",
                newName: "SaleFullReduceID");

            migrationBuilder.RenameIndex(
                name: "IX_Order_SaleFullReduceId",
                schema: "dbo",
                table: "Order",
                newName: "IX_Order_SaleFullReduceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_SaleFullReduce_SaleFullReduceID",
                schema: "dbo",
                table: "Order",
                column: "SaleFullReduceID",
                principalSchema: "dbo",
                principalTable: "SaleFullReduce",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
