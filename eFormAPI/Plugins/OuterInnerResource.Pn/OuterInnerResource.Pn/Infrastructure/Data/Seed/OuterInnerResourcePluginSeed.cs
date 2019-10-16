using System;
using System.Linq;
using Microting.eForm.Infrastructure.Constants;
using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using OuterInnerResource.Pn.Infrastructure.Data.Seed.Data;

namespace OuterInnerResource.Pn.Infrastructure.Data.Seed
{
    public class OuterInnerResourcePluginSeed
    {
        public static void SeedData(OuterInnerResourcePnDbContext dbContext)
        {
            OuterInnerResourceConfigurationSeedData seedData = new OuterInnerResourceConfigurationSeedData();
            PluginConfigurationValue[] configurationList = seedData.Data;
            foreach (PluginConfigurationValue configurationItem in configurationList)
            {
                if (!dbContext.PluginConfigurationValues.Any(x=>x.Name == configurationItem.Name))
                {
                    PluginConfigurationValue newConfigValue = new PluginConfigurationValue()
                    {
                        Name = configurationItem.Name,
                        Value = configurationItem.Value,
                        CreatedAt = DateTime.UtcNow,
                        Version = 1,
                        WorkflowState = Constants.WorkflowStates.Created,
                        CreatedByUserId = 1
                    };
                    dbContext.PluginConfigurationValues.Add(newConfigValue);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
