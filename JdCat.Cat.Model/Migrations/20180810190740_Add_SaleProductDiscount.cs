using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_SaleProductDiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleProductDiscount",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessId = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Cycle = table.Column<int>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    EndTime1 = table.Column<DateTime>(nullable: true),
                    EndTime2 = table.Column<DateTime>(nullable: true),
                    EndTime3 = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OldPrice = table.Column<decimal>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    SettingType = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    StartTime1 = table.Column<DateTime>(nullable: true),
                    StartTime2 = table.Column<DateTime>(nullable: true),
                    StartTime3 = table.Column<DateTime>(nullable: true),
                    UpperLimit = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleProductDiscount", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaleProductDiscount_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleProductDiscount_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductDiscount_BusinessId",
                schema: "dbo",
                table: "SaleProductDiscount",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductDiscount_ProductId",
                schema: "dbo",
                table: "SaleProductDiscount",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleProductDiscount",
                schema: "dbo");
        }
    }
}
