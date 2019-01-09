using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.Areas
{
    public class AreaCreateModel
    {
        public string Name { get; set; }
        public List<int> RelatedMachinesIds { get; set; } = new List<int>();
    }
}