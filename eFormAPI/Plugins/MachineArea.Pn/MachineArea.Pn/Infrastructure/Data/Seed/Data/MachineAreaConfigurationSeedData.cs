using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;

namespace MachineArea.Pn.Infrastructure.Data.Seed.Data
{
    public class MachineAreaConfigurationSeedData : IPluginConfigurationSeedData
    {
        public PluginConfigurationValue[] Data => new[]
        {
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:SdkConnectionString",
                Value = "..."
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:LogLevel",
                Value = "4"
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:LogLimit",
                Value = "25000"
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:MaxParallelism",
                Value = "1"
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:NumberOfWorkers",
                Value = "1"
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:Token",
                Value = "..."
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:SdkeFormId",
                Value = "..."
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:EnabledSiteIds",
                Value = ""
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:QuickSyncEnabled",
                Value = "false"
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:ReportTimeType",
                Value = "1"
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:OuterResourceName",
                Value = "Machines"
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:InnerResourceName",
                Value = "Areas"
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:OuterTotalTimeName",
                Value = "Working day"
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:InnerTotalTimeName",
                Value = "Time tracking for the whole day"
            },
            new PluginConfigurationValue()
            {
                Name = "MachineAreaBaseSettings:ShouldCheckAllCases",
                Value = "false"
            },
        };
    }
}
