using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaNominaAPPWeb.Data;
using SistemaNominaAPPWeb.Models.DTOs;

namespace SistemaNominaAPPWeb.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            return await _context.Employees
                .Select(e => new EmployeeDto
                {
                    EmpNo = e.EmpNo,
                    BirthDate = e.BirthDate,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Gender = e.Gender,
                    HireDate = e.HireDate,
                    Correo = e.Correo,
                    IsActive = e.IsActive
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var e = await _context.Employees.FindAsync(id);

            if (e == null) return NotFound();

            return new EmployeeDto
            {
                EmpNo = e.EmpNo,
                BirthDate = e.BirthDate,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Gender = e.Gender,
                HireDate = e.HireDate,
                Correo = e.Correo,
                IsActive = e.IsActive
            };
        }
    }
}
