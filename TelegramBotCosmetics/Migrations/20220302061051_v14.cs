using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBotCosmetics.Migrations
{
    public partial class v14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Peoples_TypeCatalogs_TypeCatalogId",
                table: "Peoples");

            migrationBuilder.DropIndex(
                name: "IX_Peoples_TypeCatalogId",
                table: "Peoples");

            migrationBuilder.DropColumn(
                name: "TypeCatalogId",
                table: "Peoples");

            migrationBuilder.CreateTable(
                name: "PeopleTypeCatalog",
                columns: table => new
                {
                    PeoplesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeCatalogsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeopleTypeCatalog", x => new { x.PeoplesId, x.TypeCatalogsId });
                    table.ForeignKey(
                        name: "FK_PeopleTypeCatalog_Peoples_PeoplesId",
                        column: x => x.PeoplesId,
                        principalTable: "Peoples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeopleTypeCatalog_TypeCatalogs_TypeCatalogsId",
                        column: x => x.TypeCatalogsId,
                        principalTable: "TypeCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeopleTypeCatalog_TypeCatalogsId",
                table: "PeopleTypeCatalog",
                column: "TypeCatalogsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeopleTypeCatalog");

            migrationBuilder.AddColumn<Guid>(
                name: "TypeCatalogId",
                table: "Peoples",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Peoples_TypeCatalogId",
                table: "Peoples",
                column: "TypeCatalogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Peoples_TypeCatalogs_TypeCatalogId",
                table: "Peoples",
                column: "TypeCatalogId",
                principalTable: "TypeCatalogs",
                principalColumn: "Id");
        }
    }
}
