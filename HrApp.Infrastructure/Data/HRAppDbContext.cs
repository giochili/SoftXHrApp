using HrApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HrApp.Infrastructure.Data
{
    public class HRAppDbContext : DbContext
    {
        public HRAppDbContext(DbContextOptions<HRAppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Position> Positions => Set<Position>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>()
                .HasOne(p => p.ParentPosition)
                .WithMany(p => p.SubPositions)
                .HasForeignKey(p => p.ParentPositionId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
