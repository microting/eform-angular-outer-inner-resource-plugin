using System.Collections.Generic;

namespace OuterInnerResource.Pn.Infrastructure.Models.OuterResources
{
    public class OuterResourceCreateModel
    {
        public string Name { get; set; }
        public int? ExternalId { get; set; }
        public List<int> RelatedInnerResourcesIds { get; set; } = new List<int>();
    }
}