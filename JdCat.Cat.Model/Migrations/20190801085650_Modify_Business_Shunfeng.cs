using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Business_Shunfeng : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShunfengDevId",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShunfengDevKey",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShunfengShopId",
                table: "Business",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShunfengDevId",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "ShunfengDevKey",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "ShunfengShopId",
                table: "Business");
        }
    }
}
