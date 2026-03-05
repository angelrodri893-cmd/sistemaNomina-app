using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaNominaAPPWeb.Models;
using static SistemaNominaAPPWeb.Models.DeptEmp;

namespace SistemaNominaAPPWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<DeptEmp> DeptEmps { get; set; } = null!;
        public DbSet<Salary> Salaries { get; set; } = null!;
        public DbSet<SalaryAudit> SalaryAudits { get; set; } = null!;
        public DbSet<DeptManager> DeptManagers { get; set; } = null!;

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

            modelBuilder.Entity<DeptManager>()
                .HasOne(d => d.Employee)
                .WithMany()
                .HasForeignKey(d => d.EmpNo);

            modelBuilder.Entity<DeptManager>()
                .HasOne(d => d.Department)
                .WithMany()
                .HasForeignKey(d => d.DeptNo);
        }
    }
}
