using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_DWD_Business : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DWDBusiness",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessId = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    addr = table.Column<string>(nullable: true),
                    city_code = table.Column<string>(nullable: true),
                    external_shopid = table.Column<string>(nullable: true),
                    lat = table.Column<long>(nullable: false),
                    lng = table.Column<long>(nullable: false),
                    mobile = table.Column<string>(nullable: true),
                    shop_title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DWDBusiness", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DWDBusiness_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DWDBusiness_BusinessId",
                schema: "dbo",
                table: "DWDBusiness",
                column: "BusinessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DWDBusiness",
                schema: "dbo");
        }
    }
}
