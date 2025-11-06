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
            var inactiveEmployees = await _context.Employees
                .Where(e => !e.IsActive && e.CreatedAt <= DateTime.UtcNow.AddHours(-1))
                .ToListAsync();

            foreach (var emp in inactiveEmployees)
                emp.IsActive = true;

            await _context.SaveChangesAsync();
        }
    }
}
