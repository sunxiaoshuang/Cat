using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Payment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentTypeObjectId",
                table: "TangOrder");

            migrationBuilder.DropColumn(
                name: "StaffObjectId",
                table: "TangOrder");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "PaymentType");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "PaymentType");

            migrationBuilder.RenameColumn(
                name: "TypeStatus",
                table: "PaymentType",
                newName: "Sort");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "PaymentType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PaymentType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentType_BusinessId",
                table: "PaymentType",
                column: "BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentType_Business_BusinessId",
                table: "PaymentType",
                column: "BusinessId",
                principalTable: "Business",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentType_Business_BusinessId",
                table: "PaymentType");

            migrationBuilder.DropIndex(
                name: "IX_PaymentType_BusinessId",
                table: "PaymentType");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "PaymentType");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PaymentType");

            migrationBuilder.RenameColumn(
                name: "Sort",
                table: "PaymentType",
                newName: "TypeStatus");

            migrationBuilder.AddColumn<string>(
                name: "PaymentTypeObjectId",
                table: "TangOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffObjectId",
                table: "TangOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "PaymentType",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Icon",
                table: "PaymentType",
                nullable: false,
                defaultValue: 0);
        }
    }
}
