using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_OpenAuthInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenAuthInfo",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    AppId = table.Column<string>(nullable: true),
                    RefreshToken = table.Column<string>(nullable: true),
                    AuthNote = table.Column<string>(nullable: true),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenAuthInfo", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OpenAuthInfo_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "OpenAuthInfo",
                columns: new[] { "ID", "AppId", "AuthNote", "BusinessId", "CreateTime", "RefreshToken" },
                values: new object[] { 1, "wxde8166b07f0baea8", null, 2, new DateTime(2018, 12, 4, 22, 18, 2, 374, DateTimeKind.Local), "refreshtoken@@@LdFvnDBdEH-o913Qta_RKX7oJfITF-35HC_nl68ADnc" });

            migrationBuilder.CreateIndex(
                name: "IX_OpenAuthInfo_BusinessId",
                schema: "dbo",
                table: "OpenAuthInfo",
                column: "BusinessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenAuthInfo",
                schema: "dbo");
        }
    }
}
