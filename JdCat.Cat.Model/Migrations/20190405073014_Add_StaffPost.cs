using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_StaffPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifyTime",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "ObjectId",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "SyncTime",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "ModifyTime",
                table: "PaymentType");

            migrationBuilder.DropColumn(
                name: "ObjectId",
                table: "PaymentType");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PaymentType");

            migrationBuilder.DropColumn(
                name: "SyncTime",
                table: "PaymentType");

            migrationBuilder.AddColumn<int>(
                name: "StaffPostId",
                table: "Staff",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StaffPost",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Authority = table.Column<int>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffPost", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StaffPost_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Staff_BusinessId",
                table: "Staff",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_StaffPostId",
                table: "Staff",
                column: "StaffPostId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffPost_BusinessId",
                table: "StaffPost",
                column: "BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Business_BusinessId",
                table: "Staff",
                column: "BusinessId",
                principalTable: "Business",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_StaffPost_StaffPostId",
                table: "Staff",
                column: "StaffPostId",
                principalTable: "StaffPost",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staff_Business_BusinessId",
                table: "Staff");

            migrationBuilder.DropForeignKey(
                name: "FK_Staff_StaffPost_StaffPostId",
                table: "Staff");

            migrationBuilder.DropTable(
                name: "StaffPost");

            migrationBuilder.DropIndex(
                name: "IX_Staff_BusinessId",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Staff_StaffPostId",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "StaffPostId",
                table: "Staff");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyTime",
                table: "Staff",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ObjectId",
                table: "Staff",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncTime",
                table: "Staff",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyTime",
                table: "PaymentType",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ObjectId",
                table: "PaymentType",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PaymentType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncTime",
                table: "PaymentType",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
