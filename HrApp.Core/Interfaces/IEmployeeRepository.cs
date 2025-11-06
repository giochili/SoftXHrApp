using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HrApp.Core.Entities;

namespace HrApp.Core.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IReadOnlyList<Employee>> SearchByNameAsync(string? query);
        Task<bool> ExistsByPersonalNumberOrEmailAsync(string personalNumber, string email, int? excludeId = null);
    }
}
