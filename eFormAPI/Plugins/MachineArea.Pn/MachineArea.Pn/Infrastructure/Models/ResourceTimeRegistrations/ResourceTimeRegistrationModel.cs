using System;

namespace MachineArea.Pn.Infrastructure.Models.ResourceTimeRegistrations
{
    public class ResourceTimeRegistrationModel
    {
        public int Id { get; set; }
        public DateTime DoneAt { get; set; }
        public string OuterResourceName { get; set; }
        public int OuterResourceId { get; set; }
        public string InnerResourceName { get; set; }
        public int InnerResourceId { get; set; }
        public int TimeInSeconds { get; set; }
        public int DoneByDeviceUserId { get; set; }
        public string DoneByDeviceUserName { get; set; }
        public int SdkCaseId { get; set; }
    }
}