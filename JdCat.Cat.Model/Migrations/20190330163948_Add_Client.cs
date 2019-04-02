using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_Client : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentType",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ObjectId = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Icon = table.Column<int>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false),
                    TypeStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ObjectId = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Alise = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: true),
                    CardId = table.Column<string>(nullable: true),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TangOrder",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ObjectId = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Identifier = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    OriginalAmount = table.Column<double>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    PeopleNumber = table.Column<int>(nullable: false),
                    MealFee = table.Column<double>(nullable: false),
                    Tips = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    OrderStatus = table.Column<int>(nullable: false),
                    OrderSource = table.Column<int>(nullable: false),
                    OrderMode = table.Column<int>(nullable: false),
                    PaymentTypeId = table.Column<int>(nullable: false),
                    PaymentTypeObjectId = table.Column<string>(nullable: true),
                    PayTime = table.Column<DateTime>(nullable: true),
                    CancelReason = table.Column<string>(nullable: true),
                    StaffObjectId = table.Column<string>(nullable: true),
                    StaffId = table.Column<int>(nullable: false),
                    DeskId = table.Column<int>(nullable: true),
                    DeskName = table.Column<string>(nullable: true),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TangOrder", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TangOrder_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TangOrder_Desk_DeskId",
                        column: x => x.DeskId,
                        principalTable: "Desk",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TangOrder_PaymentType_PaymentTypeId",
                        column: x => x.PaymentTypeId,
                        principalTable: "PaymentType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TangOrder_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TangOrderProduct",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ObjectId = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Quantity = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    OriginalPrice = table.Column<double>(nullable: false),
                    Discount = table.Column<double>(nullable: false),
                    Src = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    ProductIdSet = table.Column<string>(nullable: true),
                    Feature = table.Column<int>(nullable: false),
                    ProductStatus = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false),
                    OrderObjectId = table.Column<string>(nullable: true),
                    FormatId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TangOrderProduct", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TangOrderProduct_ProductFormat_FormatId",
                        column: x => x.FormatId,
                        principalTable: "ProductFormat",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TangOrderProduct_TangOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "TangOrder",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TangOrderProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TangOrder_BusinessId",
                table: "TangOrder",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_TangOrder_DeskId",
                table: "TangOrder",
                column: "DeskId");

            migrationBuilder.CreateIndex(
                name: "IX_TangOrder_PaymentTypeId",
                table: "TangOrder",
                column: "PaymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TangOrder_StaffId",
                table: "TangOrder",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_TangOrderProduct_FormatId",
                table: "TangOrderProduct",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_TangOrderProduct_OrderId",
                table: "TangOrderProduct",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TangOrderProduct_ProductId",
                table: "TangOrderProduct",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TangOrderProduct");

            migrationBuilder.DropTable(
                name: "TangOrder");

            migrationBuilder.DropTable(
                name: "PaymentType");

            migrationBuilder.DropTable(
                name: "Staff");
        }
    }
}
