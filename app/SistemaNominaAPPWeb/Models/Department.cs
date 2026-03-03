using System.ComponentModel.DataAnnotations;

namespace SistemaNominaAPPWeb.Models
{


    public class Department
    {
        [Key]
        public int DeptNo { get; set; }

        [Required]
        [MaxLength(50)]
        public string DeptName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
