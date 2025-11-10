using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HrApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace HrApp.Infrastructure.Jobs
{
    public class ActivateEmployeeJob : IJob

    {
        private readonly HRAppDbContext _context;

        public ActivateEmployeeJob(HRAppDbContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var employeeId = context.JobDetail.JobDataMap.GetInt("EmployeeId");
            
            if (employeeId == 0)
            {
                // Fallback: activate all employees created more than 1 hour ago (for backward compatibility)
                var inactiveEmployees = await _context.Employees
                    .Where(e => !e.IsActive && e.CreatedAt <= DateTime.UtcNow.AddHours(-1))
                    .ToListAsync();

                foreach (var emp in inactiveEmployees)
                {
                    emp.IsActive = true;
                    emp.Status = HrApp.Core.Enums.EmployeeStatus.Active;
                }
            }
            else
            {
                // Activate specific employee
                var employee = await _context.Employees.FindAsync(employeeId);
                if (employee != null && !employee.IsActive)
                {
                    employee.IsActive = true;
                    employee.Status = HrApp.Core.Enums.EmployeeStatus.Active;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
