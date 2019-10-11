using System.Threading.Tasks;
using MachineArea.Pn.Infrastructure.Models.ResourceTimeRegistrations;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Abstractions
{
    public interface IResourceTimeRegistrationService
    {
        Task<OperationDataResult<ResourceTimeRegistrationsModel>> GetAllRegistrations(int lastRegistrationId);

    }
}