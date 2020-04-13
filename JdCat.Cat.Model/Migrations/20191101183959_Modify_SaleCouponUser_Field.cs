using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_SaleCouponUser_Field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_SaleCouponUser_SaleCoupon_CouponId",
            //    table: "SaleCouponUser");

            migrationBuilder.AlterColumn<int>(
                name: "CouponId",
                table: "SaleCouponUser",
                nullable: true,
                oldClrType: typeof(int));

            //migrationBuilder.AddColumn<int>(
            //    name: "ReturnCouponId",
            //    table: "SaleCouponUser",
            //    nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponUser_ReturnCouponId",
                table: "SaleCouponUser",
                column: "ReturnCouponId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleCouponUser_SaleCoupon_CouponId",
                table: "SaleCouponUser",
                column: "CouponId",
                principalTable: "SaleCoupon",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleCouponUser_SaleReturnCoupon_ReturnCouponId",
                table: "SaleCouponUser",
                column: "ReturnCouponId",
                principalTable: "SaleReturnCoupon",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleCouponUser_SaleCoupon_CouponId",
                table: "SaleCouponUser");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleCouponUser_SaleReturnCoupon_ReturnCouponId",
                table: "SaleCouponUser");

            migrationBuilder.DropIndex(
                name: "IX_SaleCouponUser_ReturnCouponId",
                table: "SaleCouponUser");

            migrationBuilder.DropColumn(
                name: "ReturnCouponId",
                table: "SaleCouponUser");

            migrationBuilder.AlterColumn<int>(
                name: "CouponId",
                table: "SaleCouponUser",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleCouponUser_SaleCoupon_CouponId",
                table: "SaleCouponUser",
                column: "CouponId",
                principalTable: "SaleCoupon",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
