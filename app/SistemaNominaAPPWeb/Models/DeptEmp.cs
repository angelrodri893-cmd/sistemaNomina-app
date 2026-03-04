using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SistemaNominaAPPWeb.Models
{
    public class DeptEmp
    {
        public int Id { get; set; }

        [Display(Name = "Empleado")]
        public int EmpNo { get; set; }

        [Display(Name = "Departamento")]    
        public int DeptNo { get; set; }

        [Display(Name = "Desde la fecha")]
        public DateTime FromDate { get; set; }

        [Display(Name = "Hasta la fecha")]
        public DateTime? ToDate { get; set; }

        [ValidateNever] //Sirve para evitar que se valide esta propiedad, ya que no es un campo de entrada del usuario
        [Display (Name = "Empleado")]
        public Employee Employee { get; set; }

        [ValidateNever]
        [Display(Name = "Departamento")]
        public Department Department { get; set; }

        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = false;
    }
}
