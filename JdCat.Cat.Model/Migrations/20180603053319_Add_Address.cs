using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_Address : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AreaName = table.Column<string>(nullable: true),
                    CityName = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    DetailInfo = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    Lat = table.Column<double>(nullable: false),
                    Lng = table.Column<double>(nullable: false),
                    MapInfo = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    ProvinceName = table.Column<string>(nullable: true),
                    Receiver = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Address_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionData_UserId",
                schema: "dbo",
                table: "SessionData",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserId",
                schema: "dbo",
                table: "Address",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionData_User_UserId",
                schema: "dbo",
                table: "SessionData",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionData_User_UserId",
                schema: "dbo",
                table: "SessionData");

            migrationBuilder.DropTable(
                name: "Address",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_SessionData_UserId",
                schema: "dbo",
                table: "SessionData");
        }
    }
}
