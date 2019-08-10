using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Business_Eleme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Eleme_AppId",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Eleme_AppKey",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Eleme_Poi_Id",
                table: "Business",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Eleme_AppId",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "Eleme_AppKey",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "Eleme_Poi_Id",
                table: "Business");
        }
    }
}
