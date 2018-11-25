using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_OrderProduct_SetMeal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Feature",
                schema: "dbo",
                table: "OrderProduct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProductIdSet",
                schema: "dbo",
                table: "OrderProduct",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Feature",
                schema: "dbo",
                table: "OrderProduct");

            migrationBuilder.DropColumn(
                name: "ProductIdSet",
                schema: "dbo",
                table: "OrderProduct");
        }
    }
}
