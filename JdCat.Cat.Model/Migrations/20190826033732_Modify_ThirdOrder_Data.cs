using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_ThirdOrder_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Data1",
                table: "ThirdOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Data2",
                table: "ThirdOrder",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data1",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "Data2",
                table: "ThirdOrder");
        }
    }
}
