using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Product_Pinyin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstLetter",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pinyin",
                table: "Product",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstLetter",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Pinyin",
                table: "Product");
        }
    }
}
