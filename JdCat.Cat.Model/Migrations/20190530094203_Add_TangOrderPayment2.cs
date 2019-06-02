using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_TangOrderPayment2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    OrderObjectId = table.Column<string>(nullable: true),
                    TangOrderId = table.Column<int>(nullable: false)
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
                        name: "FK_TangOrderPayment_TangOrder_TangOrderId",
                        column: x => x.TangOrderId,
                        principalTable: "TangOrder",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TangOrderPayment_PaymentTypeId",
                table: "TangOrderPayment",
                column: "PaymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TangOrderPayment_TangOrderId",
                table: "TangOrderPayment",
                column: "TangOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TangOrderPayment");
        }
    }
}
