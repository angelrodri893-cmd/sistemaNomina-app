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
    public class DeptEmpsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeptEmpsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DeptEmps
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DeptEmps.Include(d => d.Department).Include(d => d.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DeptEmps/Details/5
        public async Task<IActionResult> Details(int empNo, int deptNo, DateTime fromDate)
        {
            var deptEmp = await _context.DeptEmps
                .Include(d => d.Department)
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(d =>
                    d.EmpNo == empNo &&
                    d.DeptNo == deptNo &&
                    d.FromDate == fromDate);

            if (deptEmp == null)
            {
                return NotFound();
            }

            return View(deptEmp);
        }

        // GET: DeptEmps/Create
        public IActionResult Create()
        {
            ViewData["EmpNo"] = new SelectList(_context.Employees.Where(e => e.IsActive), "EmpNo", "FirstName");
            ViewData["DeptNo"] = new SelectList(_context.Departments.Where(d => d.IsActive), "DeptNo", "DeptName");
            return View();
        }

        // POST: DeptEmps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpNo,DeptNo")] DeptEmp deptEmp)
        {
            System.Diagnostics.Debug.WriteLine("ENTRO AL POST CREATE");

            var ahora = DateTime.Now;

            deptEmp.FromDate = ahora;
            deptEmp.ToDate = DateTime.MaxValue;
            deptEmp.IsActive = true;

            try
            {
                _context.Add(deptEmp);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: " + ex.InnerException?.Message);
                System.Diagnostics.Debug.WriteLine("ERROR: " + ex.Message);
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: DeptEmps/Edit/5
        public async Task<IActionResult> Edit(int empNo, int deptNo, DateTime fromDate)
        {
            var deptEmp = await _context.DeptEmps
                .FirstOrDefaultAsync(d =>
                    d.EmpNo == empNo &&
                    d.DeptNo == deptNo &&
                    d.FromDate == fromDate);

            if (deptEmp == null)
            {
                return NotFound();
            }

            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "FirstName", deptEmp.EmpNo);
            ViewData["DeptNo"] = new SelectList(_context.Departments, "DeptNo", "DeptName", deptEmp.DeptNo);

            return View(deptEmp);
        }

        // POST: DeptEmps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int empNo, int deptNo, DateTime fromDate,
            [Bind("EmpNo,DeptNo,FromDate,ToDate,IsActive")] DeptEmp deptEmp)
        {
            if (empNo != deptEmp.EmpNo || deptNo != deptEmp.DeptNo || fromDate != deptEmp.FromDate)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deptEmp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "FirstName", deptEmp.EmpNo);
            ViewData["DeptNo"] = new SelectList(_context.Departments, "DeptNo", "DeptName", deptEmp.DeptNo);

            return View(deptEmp);
        }

        // GET: DeptEmps/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int empNo, int deptNo, DateTime fromDate)
        {
            var deptEmp = await _context.DeptEmps
                .Include(d => d.Department)
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(d =>
                    d.EmpNo == empNo &&
                    d.DeptNo == deptNo &&
                    d.FromDate == fromDate);

            if (deptEmp == null)
            {
                return NotFound();
            }

            return View(deptEmp);
        }

        // POST: DeptEmps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int empNo, int deptNo, DateTime fromDate)
        {
            var deptEmp = await _context.DeptEmps
                .FirstOrDefaultAsync(d =>
                    d.EmpNo == empNo &&
                    d.DeptNo == deptNo &&
                    d.FromDate == fromDate);

            if (deptEmp != null)
            {
                _context.DeptEmps.Remove(deptEmp);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
