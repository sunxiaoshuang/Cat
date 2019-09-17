using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_OrderActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderActivity",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ActivityId = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderActivity", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderActivity_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderActivity_OrderId",
                table: "OrderActivity",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderActivity");
        }
    }
}
