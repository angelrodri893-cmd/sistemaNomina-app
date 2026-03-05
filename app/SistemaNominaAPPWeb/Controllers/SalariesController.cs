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

namespace SistemaNominaAPPWeb.Controllers
{
    [Authorize(Roles = "Administrador,RRHH")]
    public class SalariesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalariesController(ApplicationDbContext context)
        {
            _context = context;
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
