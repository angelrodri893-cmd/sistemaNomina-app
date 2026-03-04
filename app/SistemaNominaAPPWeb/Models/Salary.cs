using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaNominaAPPWeb.Models
{
    public class Salary
    {
        [Key]
        public int SalaryId { get; set; }

        [Required]
        [Display(Name = "Empleado")]
        public int EmpNo { get; set; }

        [ForeignKey("EmpNo")]
        [Display(Name = "Empleado")]
        public Employee? Employee { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Monto")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Desde")]
        public DateTime FromDate { get; set; }

        [Display(Name = "Hasta")]
        public DateTime? ToDate { get; set; }

        [Display (Name = "Activo")]
        public bool IsActive { get; set; } = true;
    }
}
