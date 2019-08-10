using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_ChargeRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RechargeAmount",
                table: "WxMember",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "Bonus",
                table: "WxMember",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "Balance",
                table: "WxMember",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<int>(
                name: "ChargeTimes",
                table: "WxMember",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GiveAmount",
                table: "WxMember",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChargeRecord",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Amount = table.Column<int>(nullable: false),
                    Give = table.Column<int>(nullable: false),
                    Bonus = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    PrepayId = table.Column<string>(nullable: true),
                    OpenId = table.Column<string>(nullable: true),
                    WxPayCode = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    PayTime = table.Column<DateTime>(nullable: true),
                    RelativeId = table.Column<int>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeRecord", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChargeRecord_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargeRecord_BusinessId",
                table: "ChargeRecord",
                column: "BusinessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargeRecord");

            migrationBuilder.DropColumn(
                name: "ChargeTimes",
                table: "WxMember");

            migrationBuilder.DropColumn(
                name: "GiveAmount",
                table: "WxMember");

            migrationBuilder.AlterColumn<double>(
                name: "RechargeAmount",
                table: "WxMember",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<double>(
                name: "Bonus",
                table: "WxMember",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<double>(
                name: "Balance",
                table: "WxMember",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
