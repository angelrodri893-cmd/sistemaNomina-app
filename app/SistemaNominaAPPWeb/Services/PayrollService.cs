using SistemaNominaAPPWeb.Models;

namespace SistemaNominaAPPWeb.Services
{
    public interface IPayrollService
    {
        void CalculatePayroll(Salary salary);
    }

    public class PayrollService : IPayrollService
    {
        public void CalculatePayroll(Salary salary)
        {
            if (salary.Amount <= 0)
                return;

            // Constantes de descuentos definidos por requerimiento o ley
            decimal afpRate = 0.10m; // 10%
            decimal sfsRate = 0.03m; // 3%

            salary.AfpDeduction = salary.Amount * afpRate;
            salary.SfsDeduction = salary.Amount * sfsRate;

            // Renta: cálculo simplificado (escala dominicana 2024 anualizada aprox, para mensual: 34,685 exento)
            // Para simplificar, asumiremos que si gana > 35,000 se penaliza con ejemplo de 15% sobre excedente
            decimal baseGravable = salary.Amount - salary.AfpDeduction - salary.SfsDeduction;
            decimal isr = 0m;
            decimal exencionMensual = 34685.00m;

            if (baseGravable > exencionMensual)
            {
                var excedente = baseGravable - exencionMensual;

                if (baseGravable <= 52027.42m)
                {
                    isr = excedente * 0.15m;
                }
                else if (baseGravable <= 72260.25m)
                {
                    isr = (52027.42m - exencionMensual) * 0.15m + (baseGravable - 52027.42m) * 0.20m;
                }
                else
                {
                    isr = (52027.42m - exencionMensual) * 0.15m + (72260.25m - 52027.42m) * 0.20m + (baseGravable - 72260.25m) * 0.25m;
                }
            }
            
            salary.IsrDeduction = isr;

            // Salario Neto
            salary.NetSalary = salary.Amount - salary.AfpDeduction - salary.SfsDeduction - salary.IsrDeduction;
        }
    }
}
