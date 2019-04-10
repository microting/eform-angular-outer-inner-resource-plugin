using System;
using System.Linq;
using eFormShared;
using MachineArea.Pn.Infrastructure.Data.Seed.Data;
using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;
using Microting.eFormMachineAreaBase.Infrastructure.Data;

namespace MachineArea.Pn.Infrastructure.Data.Seed
{
    public class MachineAreaPluginSeed
    {
        public static void SeedData(MachineAreaPnDbContext dbContext)
        {
            var seedData = new MachineAreaConfigurationSeedData();
            var configurationList = seedData.Data;
            foreach (var configurationItem in configurationList)
            {
                if (!dbContext.PluginConfigurationValues.Any(x=>x.Name == configurationItem.Name))
                {
                    var newConfigValue = new PluginConfigurationValue()
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
