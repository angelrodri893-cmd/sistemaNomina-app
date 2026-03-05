using Microsoft.AspNetCore.Identity;

namespace SistemaNominaAPPWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? EmpNo { get; set; }
    }
}
