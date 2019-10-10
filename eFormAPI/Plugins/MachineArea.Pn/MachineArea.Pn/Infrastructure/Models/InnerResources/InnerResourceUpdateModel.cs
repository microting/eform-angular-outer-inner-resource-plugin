using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.InnerResources
{
    public class InnerResourceUpdateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RelatedOuterResourcesIds { get; set; }
    }
}