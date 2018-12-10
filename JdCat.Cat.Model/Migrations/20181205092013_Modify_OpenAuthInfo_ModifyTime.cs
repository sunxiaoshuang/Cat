using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_OpenAuthInfo_ModifyTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "OpenAuthInfo",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyTime",
                schema: "dbo",
                table: "OpenAuthInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifyTime",
                schema: "dbo",
                table: "OpenAuthInfo");

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "OpenAuthInfo",
                columns: new[] { "ID", "AppId", "AuthNote", "BusinessId", "CreateTime", "RefreshToken" },
                values: new object[] { 1, "wxde8166b07f0baea8", null, 2, new DateTime(2018, 12, 4, 22, 18, 2, 374, DateTimeKind.Local), "refreshtoken@@@LdFvnDBdEH-o913Qta_RKX7oJfITF-35HC_nl68ADnc" });
        }
    }
}
