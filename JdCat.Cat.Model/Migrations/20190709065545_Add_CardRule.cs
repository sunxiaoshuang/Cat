using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_CardRule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardBonusRule",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    Give = table.Column<double>(nullable: false),
                    Mode = table.Column<int>(nullable: false),
                    WxCardId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardBonusRule", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CardBonusRule_WxCard_WxCardId",
                        column: x => x.WxCardId,
                        principalTable: "WxCard",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardChargeRule",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    Give = table.Column<double>(nullable: false),
                    WxCardId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardChargeRule", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CardChargeRule_WxCard_WxCardId",
                        column: x => x.WxCardId,
                        principalTable: "WxCard",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardBonusRule_WxCardId",
                table: "CardBonusRule",
                column: "WxCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardChargeRule_WxCardId",
                table: "CardChargeRule",
                column: "WxCardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardBonusRule");

            migrationBuilder.DropTable(
                name: "CardChargeRule");
        }
    }
}
