using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_TangOrderPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TangOrder_PaymentType_PaymentTypeId",
                table: "TangOrder");

            migrationBuilder.DropIndex(
                name: "IX_TangOrder_PaymentTypeId",
                table: "TangOrder");

            migrationBuilder.DropColumn(
                name: "GiveAmount",
                table: "TangOrder");

            migrationBuilder.DropColumn(
                name: "PaymentTypeId",
                table: "TangOrder");

            migrationBuilder.DropColumn(
                name: "PaymentTypeName",
                table: "TangOrder");

            migrationBuilder.DropColumn(
                name: "ReceivedAmount",
                table: "TangOrder");

            migrationBuilder.CreateTable(
                name: "TangOrderPayment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ObjectId = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    SyncTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    PaymentTypeId = table.Column<int>(nullable: false),
                    PaymentTypeObjectId = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    OrderObjectId = table.Column<string>(nullable: true),
                    TangOrderID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TangOrderPayment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TangOrderPayment_PaymentType_PaymentTypeId",
                        column: x => x.PaymentTypeId,
                        principalTable: "PaymentType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TangOrderPayment_TangOrder_TangOrderID",
                        column: x => x.TangOrderID,
                        principalTable: "TangOrder",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TangOrderPayment_PaymentTypeId",
                table: "TangOrderPayment",
                column: "PaymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TangOrderPayment_TangOrderID",
                table: "TangOrderPayment",
                column: "TangOrderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TangOrderPayment");

            migrationBuilder.AddColumn<double>(
                name: "GiveAmount",
                table: "TangOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTypeId",
                table: "TangOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTypeName",
                table: "TangOrder",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ReceivedAmount",
                table: "TangOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_TangOrder_PaymentTypeId",
                table: "TangOrder",
                column: "PaymentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TangOrder_PaymentType_PaymentTypeId",
                table: "TangOrder",
                column: "PaymentTypeId",
                principalTable: "PaymentType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
