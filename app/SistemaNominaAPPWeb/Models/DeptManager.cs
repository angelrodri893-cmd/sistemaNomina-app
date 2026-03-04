using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SistemaNominaAPPWeb.Models
{
    public class DeptManager
    {
        public int Id { get; set; }   // PK simple (recomendado)

        [Display(Name = "Empleado")]
        public int EmpNo { get; set; }

        [Display(Name = "Departamento")]
        public int DeptNo { get; set; }

        [Display(Name = "Desde la fecha")]
        public DateTime FromDate { get; set; }

        [Display(Name = "Hasta la fecha")]
        public DateTime? ToDate { get; set; }

        [ValidateNever]
        public Employee Employee { get; set; }

        [ValidateNever]
        public Department Department { get; set; }
    }
}
