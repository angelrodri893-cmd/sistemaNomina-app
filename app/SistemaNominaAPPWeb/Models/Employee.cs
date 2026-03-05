using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaNominaAPPWeb.Models
{
    public class Employee
    {
        [Key]
        public int EmpNo { get; set; }

        [Required]
        [MaxLength(50)]

        public string CI { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Display(Name = "Fecha de Nacimiento")]
        public string BirthDate { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Display (Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(1)]
        [Display(Name = "Género")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Display(Name = "Fecha de Contratación")]
        public string HireDate { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Correo { get; set; } = string.Empty;

        [Display (Name = "Activo")]
        public bool IsActive { get; set; } = true; // Campo para indicar si el empleado está activo o no
    }
}

