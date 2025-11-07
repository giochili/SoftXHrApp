using System.Collections.Generic;
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
            var positions = await DbContext.Positions.AsNoTracking().ToListAsync();
            var lookup = positions.ToDictionary(p => p.Id);

            foreach (var position in positions)
            {
                position.SubPositions = new List<Position>();
            }

            var roots = new List<Position>();

            foreach (var position in positions)
            {
                if (position.ParentPositionId.HasValue && lookup.TryGetValue(position.ParentPositionId.Value, out var parent))
                {
                    parent.SubPositions!.Add(position);
                }
                else
                {
                    roots.Add(position);
                }
            }

            return roots;
        }
    }
}
