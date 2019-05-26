using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_TangOrder_ActualAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ActualAmount",
                table: "TangOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CashierName",
                table: "TangOrder",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OrderDiscount",
                table: "TangOrder",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualAmount",
                table: "TangOrder");

            migrationBuilder.DropColumn(
                name: "CashierName",
                table: "TangOrder");

            migrationBuilder.DropColumn(
                name: "OrderDiscount",
                table: "TangOrder");
        }
    }
}
