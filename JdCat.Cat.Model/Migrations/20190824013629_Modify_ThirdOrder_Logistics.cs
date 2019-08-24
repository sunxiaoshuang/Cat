using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_ThirdOrder_Logistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Error",
                table: "ThirdOrder",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LogisticsType",
                table: "ThirdOrder",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Error",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "LogisticsType",
                table: "ThirdOrder");
        }
    }
}
