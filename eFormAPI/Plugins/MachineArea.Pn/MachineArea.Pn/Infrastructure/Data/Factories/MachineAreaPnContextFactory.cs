using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MachineArea.Pn.Infrastructure.Data.Factories
{
    public class MachineAreaPnContextFactory : IDesignTimeDbContextFactory<MachineAreaPnDbContext>
    {
        public MachineAreaPnDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MachineAreaPnDbContext>();
            if (args.Any())
            {
                if (args.FirstOrDefault().ToLower().Contains("convert zero datetime"))
                {
                    optionsBuilder.UseMySql(args.FirstOrDefault());
                }
                else
                {
                    optionsBuilder.UseSqlServer(args.FirstOrDefault());
                }            }
            else
            {
                throw new ArgumentNullException("Connection string not present");
            }
            return new MachineAreaPnDbContext(optionsBuilder.Options);
        }
    }
}
