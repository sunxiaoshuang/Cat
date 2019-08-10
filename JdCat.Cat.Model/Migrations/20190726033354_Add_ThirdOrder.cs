using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_ThirdOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MT_AppId",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MT_AppKey",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MT_Poi_Id",
                table: "Business",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ThirdOrder",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    OrderId = table.Column<long>(nullable: false),
                    OrderIdView = table.Column<long>(nullable: false),
                    PoiCode = table.Column<string>(nullable: true),
                    PoiName = table.Column<string>(nullable: true),
                    PoiAddress = table.Column<string>(nullable: true),
                    PoiPhone = table.Column<string>(nullable: true),
                    RecipientName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    OrderSource = table.Column<int>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThirdOrder", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ThirdOrder_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThirdOrderActivity",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ActiveId = table.Column<int>(nullable: false),
                    ReduceFee = table.Column<double>(nullable: false),
                    ThirdCharge = table.Column<double>(nullable: false),
                    PoiCharge = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    ThirdOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThirdOrderActivity", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ThirdOrderActivity_ThirdOrder_ThirdOrderId",
                        column: x => x.ThirdOrderId,
                        principalTable: "ThirdOrder",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThirdOrderProduct",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SkuId = table.Column<string>(nullable: true),
                    Quantity = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    BoxNum = table.Column<double>(nullable: false),
                    BoxPrice = table.Column<double>(nullable: false),
                    Unit = table.Column<string>(nullable: true),
                    Discount = table.Column<double>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Spec = table.Column<string>(nullable: true),
                    CartId = table.Column<int>(nullable: false),
                    ThirdOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThirdOrderProduct", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ThirdOrderProduct_ThirdOrder_ThirdOrderId",
                        column: x => x.ThirdOrderId,
                        principalTable: "ThirdOrder",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThirdOrder_BusinessId",
                table: "ThirdOrder",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdOrderActivity_ThirdOrderId",
                table: "ThirdOrderActivity",
                column: "ThirdOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdOrderProduct_ThirdOrderId",
                table: "ThirdOrderProduct",
                column: "ThirdOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThirdOrderActivity");

            migrationBuilder.DropTable(
                name: "ThirdOrderProduct");

            migrationBuilder.DropTable(
                name: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "MT_AppId",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "MT_AppKey",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "MT_Poi_Id",
                table: "Business");
        }
    }
}
