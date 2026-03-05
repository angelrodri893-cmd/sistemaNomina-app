using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaNominaAPPWeb.Data;
using SistemaNominaAPPWeb.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaNominaAPPWeb.Controllers
{
    [Authorize(Roles = "Administrador,RRHH")]
    public class TitlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TitlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Titles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Titles.Include(t => t.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Titles/Details/5
        public async Task<IActionResult> Details(int empNo, string titleName, DateTime fromDate)
        {
            if (string.IsNullOrEmpty(titleName))
            {
                return NotFound();
            }

            var title = await _context.Titles
                .Include(t => t.Employee)
                .FirstOrDefaultAsync(m => m.EmpNo == empNo && m.TitleName == titleName && m.FromDate == fromDate);
            if (title == null)
            {
                return NotFound();
            }

            return View(title);
        }

        // GET: Titles/Create
        public IActionResult Create()
        {
            ViewData["EmpNo"] = new SelectList(_context.Employees.Where(e => e.IsActive), "EmpNo", "FirstName");
            return View();
        }

        // POST: Titles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpNo,TitleName,FromDate")] Title title)
        {
            if (ModelState.IsValid)
            {
                // Validación 1: FromDate debe ser mayor al último FromDate registrado
                var lastTitle = await _context.Titles
                    .Where(t => t.EmpNo == title.EmpNo)
                    .OrderByDescending(t => t.FromDate)
                    .FirstOrDefaultAsync();

                if (lastTitle != null && title.FromDate <= lastTitle.FromDate)
                {
                    ModelState.AddModelError("FromDate", "La fecha de inicio debe ser mayor a la del último título registrado (" + lastTitle.FromDate.ToString("dd/MM/yyyy") + ").");
                    ViewData["EmpNo"] = new SelectList(_context.Employees.Where(e => e.IsActive), "EmpNo", "FirstName", title.EmpNo);
                    return View(title);
                }

                // Cerrar automáticamente el título anterior activo si existe
                var activeTitle = await _context.Titles
                    .Where(t => t.EmpNo == title.EmpNo && t.IsActive == true)
                    .FirstOrDefaultAsync();

                if (activeTitle != null)
                {
                    activeTitle.ToDate = title.FromDate.AddDays(-1);
                    activeTitle.IsActive = false;
                    _context.Update(activeTitle);
                }

                title.IsActive = true;
                _context.Add(title);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpNo"] = new SelectList(_context.Employees.Where(e => e.IsActive), "EmpNo", "FirstName", title.EmpNo);
            return View(title);
        }

        // GET: Titles/Edit/5
        public async Task<IActionResult> Edit(int empNo, string titleName, DateTime fromDate)
        {
            if (string.IsNullOrEmpty(titleName))
            {
                return NotFound();
            }

            var title = await _context.Titles.FirstOrDefaultAsync(m => m.EmpNo == empNo && m.TitleName == titleName && m.FromDate == fromDate);
            if (title == null)
            {
                return NotFound();
            }
            ViewData["EmpNo"] = new SelectList(_context.Employees.Where(e => e.IsActive), "EmpNo", "FirstName", title.EmpNo);
            return View(title);
        }

        // POST: Titles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int empNo, string titleName, DateTime fromDate, [Bind("EmpNo,TitleName,FromDate,ToDate,IsActive")] Title title)
        {
            if (empNo != title.EmpNo || titleName != title.TitleName || fromDate != title.FromDate)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(title);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TitleExists(title.EmpNo, title.TitleName, title.FromDate))
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
            ViewData["EmpNo"] = new SelectList(_context.Employees.Where(e => e.IsActive), "EmpNo", "FirstName", title.EmpNo);
            return View(title);
        }

        // GET: Titles/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int empNo, string titleName, DateTime fromDate)
        {
            if (string.IsNullOrEmpty(titleName))
            {
                return NotFound();
            }

            var title = await _context.Titles
                .Include(t => t.Employee)
                .FirstOrDefaultAsync(m => m.EmpNo == empNo && m.TitleName == titleName && m.FromDate == fromDate);
            if (title == null)
            {
                return NotFound();
            }

            return View(title);
        }

        // POST: Titles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int empNo, string titleName, DateTime fromDate)
        {
            var title = await _context.Titles.FirstOrDefaultAsync(m => m.EmpNo == empNo && m.TitleName == titleName && m.FromDate == fromDate);
            if (title != null)
            {
                _context.Titles.Remove(title);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool TitleExists(int empNo, string titleName, DateTime fromDate)
        {
          return _context.Titles.Any(e => e.EmpNo == empNo && e.TitleName == titleName && e.FromDate == fromDate);
        }
    }
}
