using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaNominaAPPWeb.Data;
using SistemaNominaAPPWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Details(int id)
        {
            var deptEmp = await _context.DeptEmps
                .Include(d => d.Employee)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (deptEmp == null)
                return NotFound();

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
        public async Task<IActionResult> Create([Bind("EmpNo,DeptNo,FromDate,ToDate")] DeptEmp deptEmp)
        {
            if (!ModelState.IsValid)
            {
                ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "FirstName", deptEmp.EmpNo);
                ViewData["DeptNo"] = new SelectList(_context.Departments, "DeptNo", "DeptName", deptEmp.DeptNo);
                return View(deptEmp);
            }

            deptEmp.IsActive = true;

            _context.Add(deptEmp);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: DeptEmps/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var deptEmp = await _context.DeptEmps.FindAsync(id);

            if (deptEmp == null)
                return NotFound();

            return View(deptEmp);
        }

        // POST: DeptEmps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,EmpNo,DeptNo,FromDate,ToDate,IsActive")] DeptEmp deptEmp)
        {
            if (id != deptEmp.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "FirstName", deptEmp.EmpNo);
                ViewData["DeptNo"] = new SelectList(_context.Departments, "DeptNo", "DeptName", deptEmp.DeptNo);
                return View(deptEmp);
            }

            _context.Update(deptEmp);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: DeptEmps/Delete/5

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id) // Usa 'id' si es la llave primaria

        {
            if (id == null) return NotFound();

            var deptEmp = await _context.DeptEmps
                .Include(d => d.Employee)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (deptEmp == null) return NotFound();

            return View(deptEmp);
        }

        // POST: DeptEmps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id) // Aquí también usamos 'id')
        {
            var deptEmp = await _context.DeptEmps.FindAsync(id);
            if (deptEmp != null)
            {
                _context.DeptEmps.Remove(deptEmp);
                await _context.SaveChangesAsync();
            } 
            return RedirectToAction(nameof(Index));
        }
    }
}
