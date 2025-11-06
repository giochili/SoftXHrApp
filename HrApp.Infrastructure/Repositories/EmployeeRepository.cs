using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HrApp.Core.Entities;
using HrApp.Core.Interfaces;
using HrApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HrApp.Infrastructure.Repositories
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(HRAppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> ExistsByPersonalNumberOrEmailAsync(string personalNumber, string email, int? excludeId = null)
        {
            return await DbContext.Employees.AnyAsync(e =>
                (e.PersonalNumber == personalNumber || e.Email == email) &&
                (!excludeId.HasValue || e.Id != excludeId.Value));
        }

        public async Task<IReadOnlyList<Employee>> SearchByNameAsync(string? query)
        {
            if (string.IsNullOrWhiteSpace(query)) return await DbContext.Employees.Include(e => e.Position).ToListAsync();
            query = query.Trim();
            return await DbContext.Employees
                .Include(e => e.Position)
                .Where(e => EF.Functions.Like(e.FirstName, $"%{query}%") || EF.Functions.Like(e.LastName, $"%{query}%"))
                .ToListAsync();
        }
    }
}
