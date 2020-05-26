using Microsoft.EntityFrameworkCore.Migrations;

namespace CareersListing.Migrations
{
    public partial class adjustedVacancyModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SalaryScale",
                table: "Vacancies",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<string>(
                name: "JobDuration",
                table: "Vacancies",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(double),
                oldMaxLength: 20);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "SalaryScale",
                table: "Vacancies",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "JobDuration",
                table: "Vacancies",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 20);
        }
    }
}
