using System.Collections.Generic;

namespace OuterInnerResource.Pn.Infrastructure.Models.InnerResources
{
    public class InnerResourceCreateModel
    {
        public string Name { get; set; }
        public int? ExternalId { get; set; }
        public List<int> RelatedOuterResourcesIds { get; set; } = new List<int>();
    }
}