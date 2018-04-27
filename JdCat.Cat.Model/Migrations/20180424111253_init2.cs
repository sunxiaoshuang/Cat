using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Log",
                schema: "dbo",
                table: "Business",
                newName: "StoreId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppId",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessLicense",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvitationCode",
                schema: "dbo",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                schema: "dbo",
                table: "Business",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "AppId",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "BusinessLicense",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "InvitationCode",
                schema: "dbo",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "Mobile",
                schema: "dbo",
                table: "Business");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                schema: "dbo",
                table: "Business",
                newName: "Log");
        }
    }
}
