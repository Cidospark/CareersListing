using Microsoft.EntityFrameworkCore.Migrations;

namespace CareersListing.Migrations
{
    public partial class newAdjustmentMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "applicationUrl",
                table: "Vacancies",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "applicationUrl",
                table: "Vacancies");
        }
    }
}
