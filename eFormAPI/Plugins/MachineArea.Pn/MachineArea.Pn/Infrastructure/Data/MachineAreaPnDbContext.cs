using MachineArea.Pn.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MachineArea.Pn.Infrastructure.Data
{
    public class MachineAreaPnDbContext : DbContext
    {

        public MachineAreaPnDbContext() { }

        public MachineAreaPnDbContext(DbContextOptions<MachineAreaPnDbContext> options) : base(options)
        {

        }

        public DbSet<Machine> Machines { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Entities.MachineArea> MachineAreas { get; set; }
        public DbSet<MachineAreaSetting> MachineAreaSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Machine>()
                .HasIndex(x => x.Name);
            modelBuilder.Entity<Machine>()
                .HasIndex(x => x.CreatedByUserId);
            modelBuilder.Entity<Machine>()
                .HasIndex(x => x.UpdatedByUserId);
            modelBuilder.Entity<Area>()
                .HasIndex(x => x.Name);
            modelBuilder.Entity<Area>()
                .HasIndex(x => x.CreatedByUserId);
            modelBuilder.Entity<Area>()
                .HasIndex(x => x.UpdatedByUserId);
        }
    }
}
