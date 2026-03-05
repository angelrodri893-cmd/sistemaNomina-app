using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaNominaAPPWeb.Data;
using SistemaNominaAPPWeb.Models.ViewModels;

namespace SistemaNominaAPPWeb.Controllers
{
    [Authorize(Roles = "Administrador,RRHH")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ActiveEmployees()
        {
            var today = DateTime.Now;

            // Employees with IsActive = true, ordered alphabetically by FirstName then LastName
            var employees = await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();

            var reportList = new List<ActiveEmployeeReportViewModel>();

            foreach (var emp in employees)
            {
                // Get current department
                var currentDept = await _context.DeptEmps
                    .Include(d => d.Department)
                    .Where(d => d.EmpNo == emp.EmpNo && d.IsActive && d.FromDate <= today)
                    .Select(d => d.Department.DeptName)
                    .FirstOrDefaultAsync() ?? "Sin Asignar";

                // Get current salary
                var currentSalary = await _context.Salaries
                    .Where(s => s.EmpNo == emp.EmpNo && s.IsActive && s.FromDate <= today)
                    .Select(s => s.Amount)
                    .FirstOrDefaultAsync();

                reportList.Add(new ActiveEmployeeReportViewModel
                {
                    EmployeeName = $"{emp.FirstName} {emp.LastName}",
                    DepartmentName = currentDept,
                    CurrentSalary = currentSalary
                });
            }

            return View(reportList);
        }

        public async Task<IActionResult> ExportActiveEmployeesPdf()
        {
            var employees = await GetActiveEmployeesData();

            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(x => ComposeContent(x, employees));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });

            var pdfFile = document.GeneratePdf();
            return File(pdfFile, "application/pdf", $"EmpleadosActivos_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public async Task<IActionResult> ExportActiveEmployeesExcel()
        {
            var employees = await GetActiveEmployeesData();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Empleados Activos");
            
            worksheet.Cell(1, 1).Value = "Nombre del Empleado";
            worksheet.Cell(1, 2).Value = "Departamento Actual";
            worksheet.Cell(1, 3).Value = "Salario Actual";

            var header = worksheet.Range("A1:C1");
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
            header.Style.Font.FontColor = XLColor.White;

            for (int i = 0; i < employees.Count; i++)
            {
                var row = i + 2;
                var emp = employees[i];
                worksheet.Cell(row, 1).Value = emp.EmployeeName;
                worksheet.Cell(row, 2).Value = emp.DepartmentName;
                worksheet.Cell(row, 3).Value = emp.CurrentSalary;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ #,##0.00";
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"EmpleadosActivos_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Reporte de Empleados Activos").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text(text =>
                    {
                        text.Span("Generado el: ").SemiBold();
                        text.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
                });
            });
        }

        private void ComposeContent(IContainer container, List<ActiveEmployeeReportViewModel> employees)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Nombre del Empleado");
                        header.Cell().Element(CellStyle).Text("Departamento Actual");
                        header.Cell().Element(CellStyle).AlignRight().Text("Salario Actual");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var emp in employees)
                    {
                        table.Cell().Element(CellStyle).Text(emp.EmployeeName);
                        table.Cell().Element(CellStyle).Text(emp.DepartmentName);
                        table.Cell().Element(CellStyle).AlignRight().Text($"{emp.CurrentSalary:C}");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    }
                });
            });
        }

        private async Task<List<ActiveEmployeeReportViewModel>> GetActiveEmployeesData()
        {
            var today = DateTime.Now;

            var employees = await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();

            var reportList = new List<ActiveEmployeeReportViewModel>();

            foreach (var emp in employees)
            {
                var currentDept = await _context.DeptEmps
                    .Include(d => d.Department)
                    .Where(d => d.EmpNo == emp.EmpNo && d.IsActive && d.FromDate <= today)
                    .Select(d => d.Department.DeptName)
                    .FirstOrDefaultAsync() ?? "Sin Asignar";

                var currentSalary = await _context.Salaries
                    .Where(s => s.EmpNo == emp.EmpNo && s.IsActive && s.FromDate <= today)
                    .Select(s => s.Amount)
                    .FirstOrDefaultAsync();

                reportList.Add(new ActiveEmployeeReportViewModel
                {
                    EmployeeName = $"{emp.FirstName} {emp.LastName}",
                    DepartmentName = currentDept,
                    CurrentSalary = currentSalary
                });
            }

            return reportList;
        }
    }
}
