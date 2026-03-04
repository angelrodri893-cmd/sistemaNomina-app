using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaNominaAPPWeb.Migrations
{
    /// <inheritdoc />
    public partial class FixDeptEmpPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeptEmps",
                table: "DeptEmps");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeptEmps",
                table: "DeptEmps",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DeptEmps_EmpNo",
                table: "DeptEmps",
                column: "EmpNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeptEmps",
                table: "DeptEmps");

            migrationBuilder.DropIndex(
                name: "IX_DeptEmps_EmpNo",
                table: "DeptEmps");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DeptEmps");

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
        }
    }
}
