using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_SaleNewCustom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleNewCustom",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleNewCustom", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaleNewCustom_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleNewCustom_BusinessId",
                table: "SaleNewCustom",
                column: "BusinessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleNewCustom");
        }
    }
}
