namespace MachineArea.Pn.Infrastructure.Models
{
    public class MachineAreaModel
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public int AreaId { get; set; }
        public string WorkflowState { get; set; }
    }
}