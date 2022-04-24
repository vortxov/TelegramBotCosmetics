using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBotCosmetics.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Price",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<Guid>(
                name: "ItemId",
                table: "Formulas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Formulas_ItemId",
                table: "Formulas",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Formulas_Items_ItemId",
                table: "Formulas",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Formulas_Items_ItemId",
                table: "Formulas");

            migrationBuilder.DropIndex(
                name: "IX_Formulas_ItemId",
                table: "Formulas");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Formulas");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Items",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
