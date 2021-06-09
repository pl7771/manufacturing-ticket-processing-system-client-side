using Microsoft.EntityFrameworkCore.Migrations;

namespace Ticketing_System_TILE03.Migrations
{
    public partial class contractTeller : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "aantalContracten",
                schema: "Identity",
                table: "ContractType",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aantalContracten",
                schema: "Identity",
                table: "ContractType");
        }
    }
}
