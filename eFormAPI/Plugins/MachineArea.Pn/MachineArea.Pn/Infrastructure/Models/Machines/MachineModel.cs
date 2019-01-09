using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.Machines
{
    public class MachineModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RelatedAreasIds { get; set; }
    }
}
