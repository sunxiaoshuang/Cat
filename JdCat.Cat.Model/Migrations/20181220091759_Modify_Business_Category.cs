using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Business_Category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                schema: "dbo",
                table: "Business",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Business_ParentId",
                schema: "dbo",
                table: "Business",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Business_Business_ParentId",
                schema: "dbo",
                table: "Business",
                column: "ParentId",
                principalSchema: "dbo",
                principalTable: "Business",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Business_Business_ParentId",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropIndex(
                name: "IX_Business_ParentId",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "Category",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "ParentId",
                schema: "dbo",
                table: "Business");
        }
    }
}
