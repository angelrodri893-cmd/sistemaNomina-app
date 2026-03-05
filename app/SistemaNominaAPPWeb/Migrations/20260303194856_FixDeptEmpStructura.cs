using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaNominaAPPWeb.Migrations
{
    /// <inheritdoc />
    public partial class FixDeptEmpStructura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeptEmps_Employees_EmployeeId",
                table: "DeptEmps");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Correo",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeptEmps",
                table: "DeptEmps");

            migrationBuilder.DropIndex(
                name: "IX_DeptEmps_EmployeeId",
                table: "DeptEmps");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DeptEmps");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DeptEmps");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "DeptEmps",
                newName: "EmpNo");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ToDate",
                table: "DeptEmps",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeptEmps",
                table: "DeptEmps",
                columns: new[] { "EmpNo", "DeptNo", "FromDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_DeptEmps_Employees_EmpNo",
                table: "DeptEmps",
                column: "EmpNo",
                principalTable: "Employees",
                principalColumn: "EmpNo",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeptEmps_Employees_EmpNo",
                table: "DeptEmps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeptEmps",
                table: "DeptEmps");

            migrationBuilder.RenameColumn(
                name: "EmpNo",
                table: "DeptEmps",
                newName: "EmployeeId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ToDate",
                table: "DeptEmps",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DeptEmps",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DeptEmps",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeptEmps",
                table: "DeptEmps",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Correo",
                table: "Employees",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeptEmps_EmployeeId",
                table: "DeptEmps",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeptEmps_Employees_EmployeeId",
                table: "DeptEmps",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmpNo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
