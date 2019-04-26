using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_TangOrder_PreferentialAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentRemark",
                table: "TangOrder",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PreferentialAmount",
                table: "TangOrder",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentRemark",
                table: "TangOrder");

            migrationBuilder.DropColumn(
                name: "PreferentialAmount",
                table: "TangOrder");
        }
    }
}
