using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HrApp.Core.Interfaces;
using HrApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HrApp.Infrastructure.Repositories
{
    public class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly HRAppDbContext DbContext;
        protected readonly DbSet<T> DbSet;

        public RepositoryBase(HRAppDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<T>();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            DbSet.Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<IReadOnlyList<T>> ListAsync()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            DbSet.Update(entity);
            await DbContext.SaveChangesAsync();
        }
    }
}
