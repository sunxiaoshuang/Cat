using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_OrderProduct_Discount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountProductQuantity",
                schema: "dbo",
                table: "OrderProduct",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleProductDiscountId",
                schema: "dbo",
                table: "OrderProduct",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_SaleProductDiscountId",
                schema: "dbo",
                table: "OrderProduct",
                column: "SaleProductDiscountId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProduct_SaleProductDiscount_SaleProductDiscountId",
                schema: "dbo",
                table: "OrderProduct",
                column: "SaleProductDiscountId",
                principalSchema: "dbo",
                principalTable: "SaleProductDiscount",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProduct_SaleProductDiscount_SaleProductDiscountId",
                schema: "dbo",
                table: "OrderProduct");

            migrationBuilder.DropIndex(
                name: "IX_OrderProduct_SaleProductDiscountId",
                schema: "dbo",
                table: "OrderProduct");

            migrationBuilder.DropColumn(
                name: "DiscountProductQuantity",
                schema: "dbo",
                table: "OrderProduct");

            migrationBuilder.DropColumn(
                name: "SaleProductDiscountId",
                schema: "dbo",
                table: "OrderProduct");
        }
    }
}
