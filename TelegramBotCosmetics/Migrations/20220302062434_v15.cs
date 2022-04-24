using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBotCosmetics.Migrations
{
    public partial class v15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DataP",
                table: "Peoples",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataP",
                table: "Peoples");
        }
    }
}
