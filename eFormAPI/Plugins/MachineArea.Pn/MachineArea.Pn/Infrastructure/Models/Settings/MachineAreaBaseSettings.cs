using MachineArea.Pn.Infrastructure.Enums;

namespace MachineArea.Pn.Infrastructure.Models.Settings
{
    public class MachineAreaBaseSettings
    {
        public string SdkConnectionString { get; set; }
        public string LogLevel { get; set; }
        public string LogLimit { get; set; }
        public string MaxParallelism { get; set; }
        public int NumberOfWorkers { get; set; }
        public string Token { get; set; }
        public string SdkeFormId { get; set; }
        public string EnabledSiteIds { get; set; }
        public bool QuickSyncEnabled { get; set; }
        public int ReportTimeType { get; set; }
    }
}