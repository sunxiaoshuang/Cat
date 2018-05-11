using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Remove_TimeStamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "dbo",
                table: "SettingProductAttribute");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "dbo",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "dbo",
                table: "ProductImage");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "dbo",
                table: "ProductFormat");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "dbo",
                table: "ProductAttribute");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "dbo",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "dbo",
                table: "Business");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                schema: "dbo",
                table: "SettingProductAttribute",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                schema: "dbo",
                table: "ProductType",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                schema: "dbo",
                table: "ProductImage",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                schema: "dbo",
                table: "ProductFormat",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                schema: "dbo",
                table: "ProductAttribute",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                schema: "dbo",
                table: "Product",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                schema: "dbo",
                table: "Business",
                rowVersion: true,
                nullable: true);
        }
    }
}
