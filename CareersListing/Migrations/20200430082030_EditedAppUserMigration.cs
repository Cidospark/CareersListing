using Microsoft.EntityFrameworkCore.Migrations;

namespace CareersListing.Migrations
{
    public partial class EditedAppUserMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");
        }
    }
}
