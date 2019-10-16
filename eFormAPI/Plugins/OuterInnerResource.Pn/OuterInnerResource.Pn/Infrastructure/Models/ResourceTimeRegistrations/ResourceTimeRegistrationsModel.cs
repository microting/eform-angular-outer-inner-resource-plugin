using System.Collections.Generic;

namespace OuterInnerResource.Pn.Infrastructure.Models.ResourceTimeRegistrations
{
    public class ResourceTimeRegistrationsModel
    {
        public List<ResourceTimeRegistrationModel> ResourceTimeRegistrationModels { get; set; }
        public int LastResourceTimeRegistrationId { get; set; }
    }
}