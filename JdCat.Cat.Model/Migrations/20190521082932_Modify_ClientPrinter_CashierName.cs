using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_ClientPrinter_CashierName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDiscount",
                table: "Product",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CashierName",
                table: "ClientPrinter",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDiscount",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CashierName",
                table: "ClientPrinter");
        }
    }
}
