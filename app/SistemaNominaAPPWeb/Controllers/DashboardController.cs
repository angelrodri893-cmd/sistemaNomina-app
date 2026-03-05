using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaNominaAPPWeb.Data;
using SistemaNominaAPPWeb.Models.ViewModels;

namespace SistemaNominaAPPWeb.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalEmployees = await _context.Employees.CountAsync(e => e.IsActive);
            var totalDepartments = await _context.Departments.CountAsync();
            
            var today = DateTime.Now;
            var currentSalaries = await _context.Salaries
                .Where(s => s.IsActive && s.FromDate <= today)
                .Select(s => s.Amount)
                .ToListAsync();

            decimal averageSalary = currentSalaries.Any() ? currentSalaries.Average() : 0m;

            var recentChanges = await _context.Salaries
                .Include(s => s.Employee)
                .OrderByDescending(s => s.FromDate)
                .Take(5)
                .Select(s => new SalaryAuditViewModel
                {
                    EmployeeName = s.Employee.FirstName + " " + s.Employee.LastName,
                    NewSalary = s.Amount,
                    DateChanged = s.FromDate
                })
                .ToListAsync();

            // Stats for chart
            var deptsStats = await _context.Departments
                .Select(d => new DepartmentEmployeeCountViewModel
                {
                    DepartmentName = d.DeptName,
                    Count = _context.DeptEmps.Count(de => de.DeptNo == d.DeptNo && de.IsActive)
                })
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalEmployees = totalEmployees,
                TotalDepartments = totalDepartments,
                AverageSalary = averageSalary,
                RecentSalaryChanges = recentChanges,
                EmployeesPerDepartment = deptsStats
            };

            return View(viewModel);
        }
    }
}
