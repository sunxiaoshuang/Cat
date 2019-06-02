using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Drop_TangOrderPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TangOrderPayment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TangOrderPayment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    Amount = table.Column<double>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ObjectId = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    OrderObjectId = table.Column<string>(nullable: true),
                    PaymentTypeId = table.Column<int>(nullable: false),
                    PaymentTypeObjectId = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    SyncTime = table.Column<DateTime>(nullable: false),
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
    }
}
