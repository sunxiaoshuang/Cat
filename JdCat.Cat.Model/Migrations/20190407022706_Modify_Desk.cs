using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Desk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Desk_DeskType_DeskTypeId",
                table: "Desk");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "DeskType");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Desk");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Desk",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Desk_DeskType_DeskTypeId",
                table: "Desk",
                column: "DeskTypeId",
                principalTable: "DeskType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Desk_DeskType_DeskTypeId",
                table: "Desk");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Desk");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "DeskType",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Desk",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Desk_DeskType_DeskTypeId",
                table: "Desk",
                column: "DeskTypeId",
                principalTable: "DeskType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
