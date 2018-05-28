using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "StoreNumbers",
                schema: "shared");

            migrationBuilder.AlterColumn<string>(
                name: "StoreId",
                schema: "dbo",
                table: "Business",
                nullable: true,
                defaultValueSql: "'JD-' + dbo.fn_right_padding(NEXT VALUE FOR shared.StoreNumbers, 6)",
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Age = table.Column<int>(nullable: false),
                    AvatarUrl = table.Column<string>(nullable: true),
                    BusinessId = table.Column<int>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    IsPhone = table.Column<bool>(nullable: false),
                    IsRegister = table.Column<bool>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    OpenId = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_OpenId",
                schema: "dbo",
                table: "User",
                column: "OpenId",
                unique: true,
                filter: "[OpenId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "StoreNumbers",
                schema: "shared");

            migrationBuilder.AlterColumn<string>(
                name: "StoreId",
                schema: "dbo",
                table: "Business",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValueSql: "'JD-' + dbo.fn_right_padding(NEXT VALUE FOR shared.StoreNumbers, 6)");
        }
    }
}
