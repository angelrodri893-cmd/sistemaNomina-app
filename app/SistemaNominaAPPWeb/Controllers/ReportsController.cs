using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}
