using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_OrderComment_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Face",
                table: "OrderComment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "OrderComment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "OrderComment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Face",
                table: "OrderComment");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "OrderComment");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "OrderComment");
        }
    }
}
