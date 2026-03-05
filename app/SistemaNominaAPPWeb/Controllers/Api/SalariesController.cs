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
    public class SalariesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalariesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalaryDto>>> GetSalaries()
        {
            return await _context.Salaries
                .Select(s => new SalaryDto
                {
                    SalaryId = s.SalaryId,
                    EmpNo = s.EmpNo,
                    Amount = s.Amount,
                    NetSalary = s.NetSalary,
                    FromDate = s.FromDate,
                    ToDate = s.ToDate,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }
    }
}
