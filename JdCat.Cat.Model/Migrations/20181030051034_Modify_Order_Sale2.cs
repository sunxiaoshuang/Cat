using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Order_Sale2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Distance",
                schema: "dbo",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SaleCouponUserMoney",
                schema: "dbo",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SaleFullReduceMoney",
                schema: "dbo",
                table: "Order",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Distance",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "SaleCouponUserMoney",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "SaleFullReduceMoney",
                schema: "dbo",
                table: "Order");
        }
    }
}
