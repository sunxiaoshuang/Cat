using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Order_Attribute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderProductAttribute",
                schema: "dbo");

            migrationBuilder.AddColumn<int>(
                name: "FormatId",
                schema: "dbo",
                table: "ShoppingCart",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Src",
                schema: "dbo",
                table: "ShoppingCart",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_FormatId",
                schema: "dbo",
                table: "ShoppingCart",
                column: "FormatId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_ProductFormat_FormatId",
                schema: "dbo",
                table: "ShoppingCart",
                column: "FormatId",
                principalSchema: "dbo",
                principalTable: "ProductFormat",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_ProductFormat_FormatId",
                schema: "dbo",
                table: "ShoppingCart");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCart_FormatId",
                schema: "dbo",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "FormatId",
                schema: "dbo",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "Src",
                schema: "dbo",
                table: "ShoppingCart");

            migrationBuilder.CreateTable(
                name: "OrderProductAttribute",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttributeId = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    OrderProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProductAttribute", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderProductAttribute_ProductAttribute_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "dbo",
                        principalTable: "ProductAttribute",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProductAttribute_OrderProduct_OrderProductId",
                        column: x => x.OrderProductId,
                        principalSchema: "dbo",
                        principalTable: "OrderProduct",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductAttribute_AttributeId",
                schema: "dbo",
                table: "OrderProductAttribute",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductAttribute_OrderProductId",
                schema: "dbo",
                table: "OrderProductAttribute",
                column: "OrderProductId");
        }
    }
}
