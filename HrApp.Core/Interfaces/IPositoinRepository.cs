using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HrApp.Core.Entities;

namespace HrApp.Core.Interfaces
{
    public interface IPositionRepository : IRepository<Position>
    {
        Task<IReadOnlyList<Position>> GetHierarchyAsync();
    }
}
