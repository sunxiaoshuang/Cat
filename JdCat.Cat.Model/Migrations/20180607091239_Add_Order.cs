using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_Order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Freight",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoReceipt",
                schema: "dbo",
                table: "Business",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Order",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AchieveTime = table.Column<DateTime>(nullable: true),
                    BusinessId = table.Column<int>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    DeliveryMode = table.Column<int>(nullable: false),
                    Freight = table.Column<decimal>(nullable: true),
                    Lat = table.Column<double>(nullable: false),
                    Lng = table.Column<double>(nullable: false),
                    OrderCode = table.Column<string>(nullable: true, defaultValueSql: "'Cat-' + CONVERT(varchar(10), GETDATE(), 112) + dbo.fn_right_padding(NEXT VALUE FOR shared.OrderNumbers, 10)"),
                    PaymentType = table.Column<int>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: true),
                    ReceiverAddress = table.Column<string>(nullable: true),
                    ReceiverName = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    TablewareQuantity = table.Column<int>(nullable: true),
                    Tips = table.Column<decimal>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    WxPayCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Order_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Order_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrderProduct",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FormatId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    Quantity = table.Column<decimal>(nullable: true),
                    Src = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProduct", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderProduct_ProductFormat_FormatId",
                        column: x => x.FormatId,
                        principalSchema: "dbo",
                        principalTable: "ProductFormat",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrderProduct_Order_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_Order_BusinessId",
                schema: "dbo",
                table: "Order",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                schema: "dbo",
                table: "Order",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_FormatId",
                schema: "dbo",
                table: "OrderProduct",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_OrderId",
                schema: "dbo",
                table: "OrderProduct",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_ProductId",
                schema: "dbo",
                table: "OrderProduct",
                column: "ProductId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderProductAttribute",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrderProduct",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Order",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "Freight",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "IsAutoReceipt",
                schema: "dbo",
                table: "Business");
        }
    }
}
