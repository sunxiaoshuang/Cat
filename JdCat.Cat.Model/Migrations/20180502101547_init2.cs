using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishTime",
                schema: "dbo",
                table: "Product",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<int>(
                name: "Feature",
                schema: "dbo",
                table: "Product",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MinBuyQuantity",
                schema: "dbo",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyTime",
                schema: "dbo",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NotSaleTime",
                schema: "dbo",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "dbo",
                table: "Product",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                schema: "dbo",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitName",
                schema: "dbo",
                table: "Product",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductAttribute",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Item1 = table.Column<string>(nullable: true),
                    Item2 = table.Column<string>(nullable: true),
                    Item3 = table.Column<string>(nullable: true),
                    Item4 = table.Column<string>(nullable: true),
                    Item5 = table.Column<string>(nullable: true),
                    Item6 = table.Column<string>(nullable: true),
                    Item7 = table.Column<string>(nullable: true),
                    Item8 = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttribute", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductAttribute_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductFormat",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PackingPrice = table.Column<decimal>(nullable: true),
                    PackingQuantity = table.Column<decimal>(nullable: true),
                    Position = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    SKU = table.Column<string>(nullable: true),
                    Stock = table.Column<decimal>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UPC = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFormat", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductFormat_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_ProductId",
                schema: "dbo",
                table: "ProductAttribute",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFormat_ProductId",
                schema: "dbo",
                table: "ProductFormat",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAttribute",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductFormat",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "Feature",
                schema: "dbo",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MinBuyQuantity",
                schema: "dbo",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ModifyTime",
                schema: "dbo",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "NotSaleTime",
                schema: "dbo",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Tag",
                schema: "dbo",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UnitName",
                schema: "dbo",
                table: "Product");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishTime",
                schema: "dbo",
                table: "Product",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
