using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaNominaAPPWeb.Models
{
    public class SalaryAudit
    {
        [Key]
        public int AuditId { get; set; }

        public int EmpNo { get; set; }

        public decimal OldSalary { get; set; }

        public decimal NewSalary { get; set; }

        public DateTime ChangeDate { get; set; } = DateTime.Now;
    }
}