using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MachineArea.Pn.Infrastructure.Data.Factories
{
    public class MachineAreaPnContextFactory : IDesignTimeDbContextFactory<MachineAreaPnDbContext>
    {
        public MachineAreaPnDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder<MachineAreaPnDbContext>();
            if (args.Any())
            {
                optionsBuilder.UseSqlServer(args.FirstOrDefault());
            }
            else
            {
                optionsBuilder.UseSqlServer("...");
            }
            return new MachineAreaPnDbContext(optionsBuilder.Options);
        }
    }
}
