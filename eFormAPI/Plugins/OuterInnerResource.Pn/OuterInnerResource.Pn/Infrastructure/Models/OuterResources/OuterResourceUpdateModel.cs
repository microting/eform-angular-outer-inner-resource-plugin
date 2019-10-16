﻿using System.Collections.Generic;

namespace OuterInnerResource.Pn.Infrastructure.Models.OuterResources
{
    public class OuterResourceUpdateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RelatedInnerResourcesIds { get; set; } = new List<int>();
    }
}