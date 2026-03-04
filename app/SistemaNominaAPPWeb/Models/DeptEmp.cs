using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SistemaNominaAPPWeb.Models
{
    public class DeptEmp
    {
        public int Id { get; set; }

        public int EmpNo { get; set; }
        public int DeptNo { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [ValidateNever]
        public Employee Employee { get; set; }

        [ValidateNever]
        public Department Department { get; set; }

        public bool IsActive { get; set; } = false;
    }
}
