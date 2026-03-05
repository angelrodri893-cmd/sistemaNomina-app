using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SistemaNominaAPPWeb.Models
{
    public class Title
    {
        [Display(Name = "Empleado")]
        public int EmpNo { get; set; }

        [Required(ErrorMessage = "El nombre del título es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "Título")]
        public string TitleName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [Display(Name = "Desde la fecha")]
        public DateTime FromDate { get; set; }

        [Display(Name = "Hasta la fecha")]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;

        [ValidateNever]
        [Display(Name = "Empleado")]
        public Employee Employee { get; set; } = null!;
    }
}
