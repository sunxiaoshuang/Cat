using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Modify_Business_Wechat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CertFile",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewOrderTemplateId",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayServerKey",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayServerMchId",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundTemplateId",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeChatAppId",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeChatSecret",
                table: "Business",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertFile",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "NewOrderTemplateId",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "PayServerKey",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "PayServerMchId",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "RefundTemplateId",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "WeChatAppId",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "WeChatSecret",
                table: "Business");
        }
    }
}
