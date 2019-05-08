namespace MachineArea.Pn.Infrastructure.Models
{
    public class MachineAreaSiteModel
    {
        public int Id { get; set; }
        public int MachineAreaId { get; set; }
        public int MicrotingSdkSiteId { get; set; }
        public int MicrotingSdkCaseId { get; set; }
        public int MicrotingSdkeFormId { get; set; }
        public string WorkflowState { get; set; }
    }
}