using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.Machines
{
    public class MachineCreateModel
    {
        public string Name { get; set; }
        public List<int> RelatedAreasIds { get; set; } = new List<int>();
    }
}