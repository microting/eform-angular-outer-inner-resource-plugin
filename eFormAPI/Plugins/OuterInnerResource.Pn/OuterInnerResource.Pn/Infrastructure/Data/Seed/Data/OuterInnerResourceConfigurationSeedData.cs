using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;

namespace OuterInnerResource.Pn.Infrastructure.Data.Seed.Data
{
    public class OuterInnerResourceConfigurationSeedData : IPluginConfigurationSeedData
    {
        public PluginConfigurationValue[] Data => new[]
        {
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:SdkConnectionString",
                Value = "..."
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:LogLevel",
                Value = "4"
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:LogLimit",
                Value = "25000"
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:MaxParallelism",
                Value = "1"
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:NumberOfWorkers",
                Value = "1"
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:Token",
                Value = "..."
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:SdkeFormId",
                Value = "..."
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:EnabledSiteIds",
                Value = ""
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:QuickSyncEnabled",
                Value = "false"
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:ReportTimeType",
                Value = "1"
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:OuterResourceName",
                Value = "Outer resources"
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:InnerResourceName",
                Value = "Inner resources"
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:OuterTotalTimeName",
                Value = "Working day"
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:InnerTotalTimeName",
                Value = "Time tracking for the whole day"
            },
            new PluginConfigurationValue()
            {
                Name = "OuterInnerResourceSettings:ShouldCheckAllCases",
                Value = "false"
            },
        };
    }
}
