using Microsoft.EntityFrameworkCore.Migrations;

namespace CareersListing.Migrations
{
    public partial class adjustedModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vacancies_CompanyId",
                table: "Vacancies");

            migrationBuilder.AddColumn<string>(
                name: "EmployerId",
                table: "Vacancies",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_CompanyId",
                table: "Vacancies",
                column: "CompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_EmployerId",
                table: "Vacancies",
                column: "EmployerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_AspNetUsers_EmployerId",
                table: "Vacancies",
                column: "EmployerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_AspNetUsers_EmployerId",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_CompanyId",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_EmployerId",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                table: "Vacancies");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_CompanyId",
                table: "Vacancies",
                column: "CompanyId");
        }
    }
}
