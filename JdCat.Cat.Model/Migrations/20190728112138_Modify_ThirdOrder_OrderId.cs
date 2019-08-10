using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_ThirdOrder_OrderId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "ThirdOrder",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<double>(
                name: "PackageFee",
                table: "ThirdOrder",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackageFee",
                table: "ThirdOrder");

            migrationBuilder.AlterColumn<long>(
                name: "OrderId",
                table: "ThirdOrder",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
