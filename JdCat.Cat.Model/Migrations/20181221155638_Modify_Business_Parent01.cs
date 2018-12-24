using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Business_Parent01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "ParentId",
                schema: "dbo",
                table: "Business");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
