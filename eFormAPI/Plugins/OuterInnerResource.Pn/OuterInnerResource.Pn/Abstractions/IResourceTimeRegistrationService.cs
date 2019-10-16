using System.Threading.Tasks;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using OuterInnerResource.Pn.Infrastructure.Models.ResourceTimeRegistrations;

namespace OuterInnerResource.Pn.Abstractions
{
    public interface IResourceTimeRegistrationService
    {
        Task<OperationDataResult<ResourceTimeRegistrationsModel>> GetAllRegistrations(int lastRegistrationId);

    }
}