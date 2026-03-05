namespace SistemaNominaAPPWeb.Models.DTOs
{
    public class EmployeeDto
    {
        public int EmpNo { get; set; }
        public string BirthDate { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string HireDate { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class DepartmentDto
    {
        public int DeptNo { get; set; }
        public string DeptName { get; set; } = string.Empty;
    }

    public class SalaryDto
    {
        public int SalaryId { get; set; }
        public int EmpNo { get; set; }
        public decimal Amount { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsActive { get; set; }
    }
}
