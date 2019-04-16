using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Comment_Reply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReplyContent",
                table: "OrderComment",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReplyTime",
                table: "OrderComment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyContent",
                table: "OrderComment");

            migrationBuilder.DropColumn(
                name: "ReplyTime",
                table: "OrderComment");
        }
    }
}
