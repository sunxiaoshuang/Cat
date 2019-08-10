using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_ThirdOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "ThirdOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Caution",
                table: "ThirdOrder",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Ctime",
                table: "ThirdOrder",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DaySeq",
                table: "ThirdOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryTime",
                table: "ThirdOrder",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "InvoiceTitle",
                table: "ThirdOrder",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "ThirdOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "ThirdOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OriginalAmount",
                table: "ThirdOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ThirdOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientAddress",
                table: "ThirdOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientPhone",
                table: "ThirdOrder",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ShippingFee",
                table: "ThirdOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "TaxpayerId",
                table: "ThirdOrder",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Utime",
                table: "ThirdOrder",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "Caution",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "Ctime",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "DaySeq",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "DeliveryTime",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "InvoiceTitle",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "OriginalAmount",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "RecipientAddress",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "RecipientPhone",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "ShippingFee",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "TaxpayerId",
                table: "ThirdOrder");

            migrationBuilder.DropColumn(
                name: "Utime",
                table: "ThirdOrder");
        }
    }
}
