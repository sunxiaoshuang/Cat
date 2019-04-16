using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Order_Invoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceName",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceTax",
                table: "Order",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceName",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "InvoiceTax",
                table: "Order");
        }
    }
}
