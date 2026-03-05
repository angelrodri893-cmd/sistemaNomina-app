using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaNominaAPPWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollCalculations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AfpDeduction",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IsrDeduction",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetSalary",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SfsDeduction",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AfpDeduction",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "IsrDeduction",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "NetSalary",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "SfsDeduction",
                table: "Salaries");
        }
    }
}
