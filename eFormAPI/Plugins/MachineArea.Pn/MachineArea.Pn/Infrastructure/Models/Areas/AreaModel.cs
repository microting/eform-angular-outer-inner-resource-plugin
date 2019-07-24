using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.Areas
{
    public class AreaModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RelatedMachinesIds { get; set; }
    }
}
