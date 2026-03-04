using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaNominaAPPWeb.Models
{
    public class SalaryAudit
    {
        [Key]
        public int AuditId { get; set; }

        [Display(Name = "Empleado")]
        public int EmpNo { get; set; }

        [Display(Name = "Antiguo Salario")]
        public decimal OldSalary { get; set; }

        [Display (Name = "Nuevo Salario")]
        public decimal NewSalary { get; set; }

        [Display(Name = "Fecha de Cambio")]
        public DateTime ChangeDate { get; set; } = DateTime.Now;
    }
}