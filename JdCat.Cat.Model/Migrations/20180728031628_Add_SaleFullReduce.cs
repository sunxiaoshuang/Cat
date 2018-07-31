using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_SaleFullReduce : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SaleFullReduceID",
                schema: "dbo",
                table: "Order",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SaleFullReduce",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessId = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    IsForeverValid = table.Column<bool>(nullable: false),
                    MinPrice = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ReduceMoney = table.Column<decimal>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleFullReduce", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaleFullReduce_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_SaleFullReduceID",
                schema: "dbo",
                table: "Order",
                column: "SaleFullReduceID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleFullReduce_BusinessId",
                schema: "dbo",
                table: "SaleFullReduce",
                column: "BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_SaleFullReduce_SaleFullReduceID",
                schema: "dbo",
                table: "Order",
                column: "SaleFullReduceID",
                principalSchema: "dbo",
                principalTable: "SaleFullReduce",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_SaleFullReduce_SaleFullReduceID",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropTable(
                name: "SaleFullReduce",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Order_SaleFullReduceID",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "SaleFullReduceID",
                schema: "dbo",
                table: "Order");
        }
    }
}
