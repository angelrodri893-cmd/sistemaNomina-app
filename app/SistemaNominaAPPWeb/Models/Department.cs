using System.ComponentModel.DataAnnotations;

namespace SistemaNominaAPPWeb.Models
{


    public class Department
    {
        [Key]
        public int DeptNo { get; set; }

        [Required]
        [MaxLength(50)]
        [Display (Name = "Nombre del Departamento")]
        public string DeptName { get; set; } = string.Empty;

        [Display (Name = "Activo")]
        public bool IsActive { get; set; } = true;
    }
}
