    using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaNominaAPPWeb.Models
{
    public class DeptEmp
    {
        public int EmpNo { get; set; }
        public int DeptNo { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public Employee Employee { get; set; }

        public Department Department { get; set; }

        public bool IsActive { get; set; } = false;
    }
}
