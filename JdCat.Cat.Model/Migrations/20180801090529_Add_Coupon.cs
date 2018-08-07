using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_Coupon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleCoupon",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessId = table.Column<int>(nullable: false),
                    Consumed = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDelete = table.Column<bool>(nullable: false),
                    MinConsume = table.Column<double>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Received = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    Stock = table.Column<int>(nullable: false),
                    ValidDay = table.Column<int>(nullable: true),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCoupon", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaleCoupon_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleCouponUser",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CouponId = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    MinConsume = table.Column<double>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    ValidDay = table.Column<int>(nullable: true),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCouponUser", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaleCouponUser_SaleCoupon_CouponId",
                        column: x => x.CouponId,
                        principalSchema: "dbo",
                        principalTable: "SaleCoupon",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleCouponUser_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleCoupon_BusinessId",
                schema: "dbo",
                table: "SaleCoupon",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponUser_CouponId",
                schema: "dbo",
                table: "SaleCouponUser",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponUser_UserId",
                schema: "dbo",
                table: "SaleCouponUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleCouponUser",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SaleCoupon",
                schema: "dbo");
        }
    }
}
