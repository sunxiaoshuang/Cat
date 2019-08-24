using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Business_MT_IsDelivery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MT_DeliveryMode",
                table: "Business",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MT_IsDelivery",
                table: "Business",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MT_DeliveryMode",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "MT_IsDelivery",
                table: "Business");
        }
    }
}
