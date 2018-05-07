using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Web.Migrations
{
    public partial class InitCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Business",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    AppId = table.Column<string>(nullable: true),
                    BusinessLicense = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    InvitationCode = table.Column<string>(nullable: true),
                    Mobile = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    RegisterDate = table.Column<DateTime>(nullable: true),
                    StoreId = table.Column<string>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProductType",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessId = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Sort = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductType", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductType_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessId = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Feature = table.Column<int>(nullable: false),
                    MinBuyQuantity = table.Column<decimal>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    NotSaleTime = table.Column<DateTime>(nullable: true),
                    ProductTypeId = table.Column<int>(nullable: true),
                    PublishTime = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Tag = table.Column<string>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UnitName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Product_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_ProductType_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalSchema: "dbo",
                        principalTable: "ProductType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "ProductImage",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ExtensionName = table.Column<string>(nullable: true),
                    Length = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImage", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductImage_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_BusinessId",
                schema: "dbo",
                table: "Product",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductTypeId",
                schema: "dbo",
                table: "Product",
                column: "ProductTypeId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                schema: "dbo",
                table: "ProductImage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductType_BusinessId",
                schema: "dbo",
                table: "ProductType",
                column: "BusinessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAttribute",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductFormat",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductImage",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Product",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductType",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Business",
                schema: "dbo");
        }
    }
}
