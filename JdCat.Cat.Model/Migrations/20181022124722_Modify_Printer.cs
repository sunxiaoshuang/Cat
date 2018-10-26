using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Printer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                schema: "dbo",
                table: "FeyinDevice",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "dbo",
                table: "FeyinDevice",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AppQrCode",
                schema: "dbo",
                table: "Business",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                schema: "dbo",
                table: "FeyinDevice");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "dbo",
                table: "FeyinDevice");

            migrationBuilder.DropColumn(
                name: "AppQrCode",
                schema: "dbo",
                table: "Business");
        }
    }
}
