using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.InnerResources
{
    public class InnerResourceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RelatedOuterResourcesIds { get; set; }
    }
}
