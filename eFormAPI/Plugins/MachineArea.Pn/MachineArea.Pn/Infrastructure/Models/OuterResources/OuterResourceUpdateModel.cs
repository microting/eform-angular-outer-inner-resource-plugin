using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.OuterResources
{
    public class OuterResourceUpdateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RelatedMachinesIds { get; set; } = new List<int>();
    }
}