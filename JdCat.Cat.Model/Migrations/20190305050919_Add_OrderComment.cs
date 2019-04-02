using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_OrderComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentResult",
                table: "OrderProduct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderCommentId",
                table: "OrderProduct",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImageWarehouse",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    RelativeId = table.Column<int>(nullable: false),
                    ImageType = table.Column<int>(nullable: false),
                    Src = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageWarehouse", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "OrderComment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ArrivedTime = table.Column<DateTime>(nullable: false),
                    ActualTime = table.Column<DateTime>(nullable: true),
                    DeliveryScore = table.Column<int>(nullable: false),
                    DeliveryType = table.Column<int>(nullable: false),
                    DeliveryResult = table.Column<string>(nullable: true),
                    CommentContent = table.Column<string>(nullable: true),
                    CommentResult = table.Column<string>(nullable: true),
                    OrderScore = table.Column<int>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderComment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderComment_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderComment_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderComment_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_OrderCommentId",
                table: "OrderProduct",
                column: "OrderCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderComment_BusinessId",
                table: "OrderComment",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderComment_OrderId",
                table: "OrderComment",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderComment_UserId",
                table: "OrderComment",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProduct_OrderComment_OrderCommentId",
                table: "OrderProduct",
                column: "OrderCommentId",
                principalTable: "OrderComment",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProduct_OrderComment_OrderCommentId",
                table: "OrderProduct");

            migrationBuilder.DropTable(
                name: "ImageWarehouse");

            migrationBuilder.DropTable(
                name: "OrderComment");

            migrationBuilder.DropIndex(
                name: "IX_OrderProduct_OrderCommentId",
                table: "OrderProduct");

            migrationBuilder.DropColumn(
                name: "CommentResult",
                table: "OrderProduct");

            migrationBuilder.DropColumn(
                name: "OrderCommentId",
                table: "OrderProduct");
        }
    }
}
