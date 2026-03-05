using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaNominaAPPWeb.Models
{
    public class Salary
    {
        [Key]
        public int SalaryId { get; set; }

        [Required]
        public int EmpNo { get; set; }

        [ForeignKey("EmpNo")]
        public Employee? Employee { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
