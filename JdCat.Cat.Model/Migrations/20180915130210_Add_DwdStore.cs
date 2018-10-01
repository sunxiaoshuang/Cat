using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_DwdStore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DWDBusiness",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "DWDStore",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessId = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    addr = table.Column<string>(nullable: true),
                    city_code = table.Column<string>(nullable: true),
                    city_name = table.Column<string>(nullable: true),
                    external_shopid = table.Column<string>(nullable: true),
                    lat = table.Column<long>(nullable: false),
                    lng = table.Column<long>(nullable: false),
                    mobile = table.Column<string>(nullable: true),
                    shop_title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DWDStore", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DWDStore_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DWD_Recharges",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<double>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    DWD_BusinessId = table.Column<int>(nullable: false),
                    DwdCode = table.Column<string>(nullable: true),
                    IsFinish = table.Column<bool>(nullable: false),
                    Mode = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DWD_Recharges", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DWD_Recharges_DWDStore_DWD_BusinessId",
                        column: x => x.DWD_BusinessId,
                        principalSchema: "dbo",
                        principalTable: "DWDStore",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DWD_Recharges_DWD_BusinessId",
                table: "DWD_Recharges",
                column: "DWD_BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_DWDStore_BusinessId",
                schema: "dbo",
                table: "DWDStore",
                column: "BusinessId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DWD_Recharges");

            migrationBuilder.DropTable(
                name: "DWDStore",
                schema: "dbo");

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
                    city_name = table.Column<string>(nullable: true),
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
    }
}
