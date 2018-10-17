using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_WxListenUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpendId",
                schema: "dbo",
                table: "Business");

            migrationBuilder.CreateTable(
                name: "WxListenUser",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessId = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    city = table.Column<string>(nullable: true),
                    country = table.Column<string>(nullable: true),
                    headimgurl = table.Column<string>(nullable: true),
                    nickname = table.Column<string>(nullable: true),
                    openid = table.Column<string>(nullable: true),
                    province = table.Column<string>(nullable: true),
                    sex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WxListenUser", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WxListenUser_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WxListenUser_BusinessId",
                schema: "dbo",
                table: "WxListenUser",
                column: "BusinessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WxListenUser",
                schema: "dbo");

            migrationBuilder.AddColumn<string>(
                name: "OpendId",
                schema: "dbo",
                table: "Business",
                nullable: true);
        }
    }
}
