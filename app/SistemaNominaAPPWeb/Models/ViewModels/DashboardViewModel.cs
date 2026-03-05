namespace SistemaNominaAPPWeb.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public decimal AverageSalary { get; set; }
        public List<SalaryAuditViewModel> RecentSalaryChanges { get; set; } = new List<SalaryAuditViewModel>();
        public List<DepartmentEmployeeCountViewModel> EmployeesPerDepartment { get; set; } = new List<DepartmentEmployeeCountViewModel>();
    }

    public class SalaryAuditViewModel
    {
        public string EmployeeName { get; set; } = string.Empty;
        public decimal NewSalary { get; set; }
        public DateTime DateChanged { get; set; }
    }

    public class DepartmentEmployeeCountViewModel
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
