using Microsoft.EntityFrameworkCore;
using SistemaNominaAPPWeb.Models;
using static SistemaNominaAPPWeb.Models.DeptEmp;

namespace SistemaNominaAPPWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DeptEmp> DeptEmps { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<SalaryAudit> SalaryAudits { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SalaryAudit>()
                .Property(s => s.OldSalary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalaryAudit>()
                .Property(s => s.NewSalary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DeptEmp>()
                .HasOne(d => d.Employee)
                .WithMany()
                .HasForeignKey(d => d.EmpNo);

            modelBuilder.Entity<DeptEmp>()
                .HasOne(d => d.Department)
                .WithMany()
                .HasForeignKey(d => d.DeptNo);
        }
    }
}
