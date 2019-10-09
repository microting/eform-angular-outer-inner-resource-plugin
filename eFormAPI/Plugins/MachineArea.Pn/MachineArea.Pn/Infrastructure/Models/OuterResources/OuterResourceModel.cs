using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.OuterResources
{
    public class OuterResourceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RelatedMachinesIds { get; set; }
    }
}
