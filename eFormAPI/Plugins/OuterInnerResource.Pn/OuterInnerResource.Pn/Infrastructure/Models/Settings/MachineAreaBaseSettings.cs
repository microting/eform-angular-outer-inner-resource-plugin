namespace OuterInnerResource.Pn.Infrastructure.Models.Settings
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
        public string OuterResourceName { get; set; }
        public string InnerResourceName { get; set; }
        public string OuterTotalTimeName { get; set; }
        public string InnerTotalTimeName { get; set; }
        public bool ShouldCheckAllCases { get; set; }
    }
}