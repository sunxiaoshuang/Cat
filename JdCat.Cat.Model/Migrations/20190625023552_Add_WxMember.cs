using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_WxMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WxMember",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    Bonus = table.Column<double>(nullable: false),
                    RechargeAmount = table.Column<double>(nullable: false),
                    PurchaseTimes = table.Column<int>(nullable: false),
                    CardId = table.Column<string>(nullable: true),
                    WxCardId = table.Column<int>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WxMember", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WxMember_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WxMember_WxCard_WxCardId",
                        column: x => x.WxCardId,
                        principalTable: "WxCard",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WxMember_BusinessId",
                table: "WxMember",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_WxMember_WxCardId",
                table: "WxMember",
                column: "WxCardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WxMember");
        }
    }
}
