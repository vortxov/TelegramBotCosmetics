using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBotCosmetics.Migrations
{
    public partial class v11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "People",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "People",
                table: "Catalogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TypeId",
                table: "Catalogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "TypeCatalogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeCatalogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_TypeId",
                table: "Catalogs",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogs_TypeCatalogs_TypeId",
                table: "Catalogs",
                column: "TypeId",
                principalTable: "TypeCatalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalogs_TypeCatalogs_TypeId",
                table: "Catalogs");

            migrationBuilder.DropTable(
                name: "TypeCatalogs");

            migrationBuilder.DropIndex(
                name: "IX_Catalogs_TypeId",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "People",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "People",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Catalogs");
        }
    }
}
