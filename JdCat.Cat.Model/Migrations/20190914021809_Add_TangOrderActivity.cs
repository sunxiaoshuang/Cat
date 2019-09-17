using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class Add_TangOrderActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TangOrderActivity",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ObjectId = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    SyncTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ActivityId = table.Column<int>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    TangOrderObjectId = table.Column<string>(nullable: true),
                    TangOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TangOrderActivity", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TangOrderActivity_TangOrder_TangOrderId",
                        column: x => x.TangOrderId,
                        principalTable: "TangOrder",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TangOrderActivity_TangOrderId",
                table: "TangOrderActivity",
                column: "TangOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TangOrderActivity");
        }
    }
}
