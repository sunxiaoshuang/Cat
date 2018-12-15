using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Update_Order_Refund : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                schema: "dbo",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundNo",
                schema: "dbo",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundReason",
                schema: "dbo",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefundStatus",
                schema: "dbo",
                table: "Order",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelReason",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RefundNo",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RefundReason",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RefundStatus",
                schema: "dbo",
                table: "Order");
        }
    }
}
