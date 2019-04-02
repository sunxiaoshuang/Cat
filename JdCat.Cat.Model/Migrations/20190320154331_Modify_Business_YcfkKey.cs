using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Business_YcfkKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YcfkKey",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YcfkSecret",
                table: "Business",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YcfkKey",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "YcfkSecret",
                table: "Business");
        }
    }
}
