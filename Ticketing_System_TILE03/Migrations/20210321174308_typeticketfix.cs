using Microsoft.EntityFrameworkCore.Migrations;

namespace Ticketing_System_TILE03.Migrations
{
    public partial class typeticketfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "Identity",
                table: "Ticket");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "Identity",
                table: "Ticket",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
