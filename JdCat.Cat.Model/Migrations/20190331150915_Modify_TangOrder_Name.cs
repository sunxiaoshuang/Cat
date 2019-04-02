using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_TangOrder_Name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentTypeName",
                table: "TangOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffName",
                table: "TangOrder",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentTypeName",
                table: "TangOrder");

            migrationBuilder.DropColumn(
                name: "StaffName",
                table: "TangOrder");
        }
    }
}
