using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Factories;

namespace OuterInnerResource.Pn.Infrastructure.Helpers
{
    public class DbContextHelper
    {
        private string ConnectionString { get;}

        public DbContextHelper(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public OuterInnerResourcePnDbContext GetDbContext()
        {
            OuterInnerResourcePnContextFactory contextFactory = new OuterInnerResourcePnContextFactory();

            return contextFactory.CreateDbContext(new[] { ConnectionString });
        }
    }
}