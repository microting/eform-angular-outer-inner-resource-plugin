using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.InnerResources
{
    public class InnerResourceCreateModel
    {
        public string Name { get; set; }
        public List<int> RelatedAreasIds { get; set; } = new List<int>();
    }
}