using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_Dada2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DadaShopNo",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DadaSourceId",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DadaCallBack",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    cancel_from = table.Column<int>(nullable: false),
                    cancel_reason = table.Column<string>(nullable: true),
                    client_id = table.Column<string>(nullable: true),
                    dm_id = table.Column<int>(nullable: false),
                    dm_mobile = table.Column<string>(nullable: true),
                    dm_name = table.Column<string>(nullable: true),
                    order_id = table.Column<string>(nullable: true),
                    order_status = table.Column<int>(nullable: false),
                    signature = table.Column<string>(nullable: true),
                    update_time = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadaCallBack", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DadaCallBack_Order_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DadaReturn",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CouponFee = table.Column<double>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    DeliverFee = table.Column<double>(nullable: false),
                    Distance = table.Column<double>(nullable: false),
                    Fee = table.Column<double>(nullable: false),
                    InsuranceFee = table.Column<double>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    Tips = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadaReturn", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DadaReturn_Order_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DadaCallBack_OrderId",
                schema: "dbo",
                table: "DadaCallBack",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DadaReturn_OrderId",
                schema: "dbo",
                table: "DadaReturn",
                column: "OrderId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DadaCallBack",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DadaReturn",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "DadaShopNo",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "DadaSourceId",
                schema: "dbo",
                table: "Business");
        }
    }
}
