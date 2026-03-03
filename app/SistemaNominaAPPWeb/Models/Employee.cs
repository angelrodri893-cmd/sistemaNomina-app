namespace SistemaNominaAPPWeb.Models
{
    // Modelo de datos para la tabla "Employees"
    using System.ComponentModel.DataAnnotations;
    public class Employee
    {
        [Key]
        public int EmpNo { get; set; }

        [Required]
        [MaxLength(50)]
        public string CI { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string BirthDate { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(1)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string HireDate { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Correo { get; set; } = string.Empty;


        public bool IsActive { get; set; } = true; // Campo para indicar si el empleado está activo o no
    }
}

