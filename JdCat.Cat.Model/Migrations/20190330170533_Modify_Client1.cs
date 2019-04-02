using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Client1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TangOrder_Staff_StaffId",
                table: "TangOrder");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "TangOrder",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_TangOrder_Staff_StaffId",
                table: "TangOrder",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TangOrder_Staff_StaffId",
                table: "TangOrder");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "TangOrder",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TangOrder_Staff_StaffId",
                table: "TangOrder",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
