using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SistemaNominaAPPWeb.Data;
using SistemaNominaAPPWeb.Models;
using SistemaNominaAPPWeb.Services;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace SistemaNominaAPPWeb.Controllers
{
    [Authorize(Roles = "Administrador,RRHH")]
    public class SalariesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPayrollService _payrollService;

        public SalariesController(ApplicationDbContext context, IPayrollService payrollService)
        {
            _context = context;
            _payrollService = payrollService;
        }

        // GET: Salaries
        public async Task<IActionResult> Index()
        {
            var salaries = _context.Salaries
                .Include(s => s.Employee)
                .Where(s => s.IsActive);

            return View(await salaries.ToListAsync());
        }

        // GET: Salaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.SalaryId == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // GET: Salaries/Create
        public IActionResult Create()
        {
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "BirthDate");
            return View();
        }

        // POST: Salaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalaryId,EmpNo,Amount,FromDate,ToDate,IsActive")] Salary salary)
        {
            if (ModelState.IsValid)
            {
                var existingSalary = await _context.Salaries
                    .Where(s => s.EmpNo == salary.EmpNo && s.IsActive)
                    .OrderByDescending(s => s.FromDate)
                    .FirstOrDefaultAsync();

                if (existingSalary != null)
                {
                    if (salary.FromDate <= existingSalary.FromDate)
                    {
                        ModelState.AddModelError("", "La fecha debe ser mayor al último salario registrado.");
                        return View(salary);
                    }

                    // 🔹 AUDITORÍA AQUÍ
                    var audit = new SalaryAudit
                    {
                        EmpNo = salary.EmpNo,
                        OldSalary = existingSalary.Amount,
                        NewSalary = salary.Amount,
                        ChangeDate = DateTime.Now
                    };

                    _context.SalaryAudits.Add(audit);

                    existingSalary.ToDate = salary.FromDate.AddDays(-1);
                    existingSalary.IsActive = false;

                    _context.Update(existingSalary);
                }

                salary.IsActive = true;
                _payrollService.CalculatePayroll(salary);

                _context.Add(salary);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(salary);
        }

        // GET: Salaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries.FindAsync(id);
            if (salary == null)
            {
                return NotFound();
            }
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "BirthDate", salary.EmpNo);
            return View(salary);
        }

        // POST: Salaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SalaryId,EmpNo,Amount,FromDate,ToDate,IsActive")] Salary salary)
        {
            if (id != salary.SalaryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryExists(salary.SalaryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "BirthDate", salary.EmpNo);
            return View(salary);
        }

        // GET: Salaries/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.SalaryId == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // POST: Salaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salary = await _context.Salaries.FindAsync(id);
            if (salary != null)
            {
                _context.Salaries.Remove(salary);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryExists(int id)
        {
            return _context.Salaries.Any(e => e.SalaryId == id);
        }

        public async Task<IActionResult> ExportHistoryPdf(int empNo)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmpNo == empNo);
            if (employee == null) return NotFound();

            var salaries = await _context.Salaries
                .Include(s => s.Employee)
                .Where(s => s.EmpNo == empNo)
                .OrderByDescending(s => s.FromDate)
                .ToListAsync();

            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(x => ComposeHeader(x, employee));
                    page.Content().Element(x => ComposeContent(x, salaries));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });

            var pdfFile = document.GeneratePdf();
            return File(pdfFile, "application/pdf", $"Salarios_{empNo}.pdf");
        }

        public async Task<IActionResult> ExportSalariesExcel()
        {
            var salaries = await _context.Salaries
                .Include(s => s.Employee)
                .OrderBy(s => s.Employee.FirstName)
                .ThenByDescending(s => s.FromDate)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Historial Salarial");

            var headers = new[] { "Empleado", "Monto", "Neto", "AFP", "SFS", "ISR", "Fecha Desde", "Fecha Hasta", "Activo" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRow = worksheet.Range("A1:I1");
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.Teal;
            headerRow.Style.Font.FontColor = XLColor.White;

            for (int i = 0; i < salaries.Count; i++)
            {
                var row = i + 2;
                var sal = salaries[i];
                worksheet.Cell(row, 1).Value = $"{sal.Employee.FirstName} {sal.Employee.LastName}";
                worksheet.Cell(row, 2).Value = sal.Amount;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "$ #,##0.00";
                worksheet.Cell(row, 3).Value = sal.NetSalary;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "$ #,##0.00";
                worksheet.Cell(row, 4).Value = sal.AfpDeduction;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "$ #,##0.00";
                worksheet.Cell(row, 5).Value = sal.SfsDeduction;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "$ #,##0.00";
                worksheet.Cell(row, 6).Value = sal.IsrDeduction;
                worksheet.Cell(row, 6).Style.NumberFormat.Format = "$ #,##0.00";
                worksheet.Cell(row, 7).Value = sal.FromDate.ToShortDateString();
                worksheet.Cell(row, 8).Value = sal.ToDate?.ToShortDateString() ?? "Actualidad";
                worksheet.Cell(row, 9).Value = sal.IsActive ? "Sí" : "No";
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"HistorialSalarial_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        private void ComposeHeader(IContainer container, Employee employee)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Historial Salarial: {employee.FirstName} {employee.LastName}").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text(text =>
                    {
                        text.Span("Generado el: ").SemiBold();
                        text.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
                });
            });
        }

        private void ComposeContent(IContainer container, List<Salary> salaries)
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
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Monto");
                        header.Cell().Element(CellStyle).Text("Salario Neto");
                        header.Cell().Element(CellStyle).Text("Desde");
                        header.Cell().Element(CellStyle).Text("Hasta");
                        header.Cell().Element(CellStyle).Text("Activo");

                        static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    });

                    foreach (var sal in salaries)
                    {
                        table.Cell().Element(CellStyle).Text($"{sal.Amount:C}");
                        table.Cell().Element(CellStyle).Text($"{sal.NetSalary:C}");
                        table.Cell().Element(CellStyle).Text(sal.FromDate.ToShortDateString());
                        table.Cell().Element(CellStyle).Text(sal.ToDate?.ToShortDateString() ?? "N/A");
                        table.Cell().Element(CellStyle).Text(sal.IsActive ? "Sí" : "No");

                        static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                });
            });
        }

        public async Task<IActionResult> History(int empNo)
        {
            var salaries = await _context.Salaries
                .Where(s => s.EmpNo == empNo)
                .OrderByDescending(s => s.FromDate)
                .ToListAsync();

            return View(salaries);
        }
    }
}
