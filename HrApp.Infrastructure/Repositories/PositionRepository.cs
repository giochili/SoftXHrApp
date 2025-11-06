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
    public class PositionRepository : RepositoryBase<Position>, IPositionRepository
    {
        public PositionRepository(HRAppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Position>> GetHierarchyAsync()
        {
            // Load full tree (simple approach)
            return await DbContext.Positions
                .Include(p => p.SubPositions)
                .ToListAsync();
        }
    }
}
