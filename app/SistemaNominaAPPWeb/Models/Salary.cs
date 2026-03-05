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
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Monto")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Desde al fecha")]
        public DateTime FromDate { get; set; }

        [Display(Name = "Hasta la fecha")]
        public DateTime? ToDate { get; set; }

        [Display (Name = "Activo")]
        public bool IsActive { get; set; } = true;

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AfpDeduction { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SfsDeduction { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal IsrDeduction { get; set; }
    }
}
