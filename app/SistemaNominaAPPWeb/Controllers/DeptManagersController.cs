using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaNominaAPPWeb.Data;
using SistemaNominaAPPWeb.Models;

namespace SistemaNominaAPPWeb.Controllers
{
    public class DeptManagersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeptManagersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DeptManagers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DeptManagers.Include(d => d.Department).Include(d => d.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DeptManagers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deptManager = await _context.DeptManagers
                .Include(d => d.Department)
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deptManager == null)
            {
                return NotFound();
            }

            return View(deptManager);
        }

        // GET: DeptManagers/Create
        public IActionResult Create()
        {
            ViewData["DeptNo"] = new SelectList(_context.Departments, "DeptNo", "DeptName");
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "FirstName");
            return View();
        }

        // POST: DeptManagers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmpNo,DeptNo,FromDate,ToDate")] DeptManager deptManager)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deptManager);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeptNo"] = new SelectList(_context.Departments, "DeptNo", "DeptName", deptManager.DeptNo);
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "FirstName", deptManager.EmpNo);
            return View(deptManager);
        }

        // GET: DeptManagers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deptManager = await _context.DeptManagers.FindAsync(id);
            if (deptManager == null)
            {
                return NotFound();
            }
            ViewData["DeptNo"] = new SelectList(_context.Departments, "DeptNo", "DeptName", deptManager.DeptNo);
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "FirstName", deptManager.EmpNo);
            return View(deptManager);
        }

        // POST: DeptManagers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmpNo,DeptNo,FromDate,ToDate")] DeptManager deptManager)
        {
            if (id != deptManager.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deptManager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeptManagerExists(deptManager.Id))
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
            ViewData["DeptNo"] = new SelectList(_context.Departments, "DeptNo", "DeptName", deptManager.DeptNo);
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "FirstName", deptManager.EmpNo);
            return View(deptManager);
        }

        // GET: DeptManagers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deptManager = await _context.DeptManagers
                .Include(d => d.Department)
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deptManager == null)
            {
                return NotFound();
            }

            return View(deptManager);
        }

        // POST: DeptManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deptManager = await _context.DeptManagers.FindAsync(id);
            if (deptManager != null)
            {
                _context.DeptManagers.Remove(deptManager);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeptManagerExists(int id)
        {
            return _context.DeptManagers.Any(e => e.Id == id);
        }
    }
}
