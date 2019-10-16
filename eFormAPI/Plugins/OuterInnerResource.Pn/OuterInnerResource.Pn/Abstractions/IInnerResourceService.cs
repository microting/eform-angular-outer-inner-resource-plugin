using System.Threading.Tasks;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using OuterInnerResource.Pn.Infrastructure.Models.InnerResources;

namespace OuterInnerResource.Pn.Abstractions
{
    public interface IInnerResourceService
    {
        Task<OperationResult> CreateMachine(InnerResourceModel model);
        Task<OperationResult> DeleteMachine(int machineId);
        Task<OperationDataResult<InnerResourcesModel>> GetAllMachines(InnerResourceRequestModel requestModel);
        Task<OperationDataResult<InnerResourceModel>> GetSingleMachine(int machineId);
        Task<OperationResult> UpdateMachine(InnerResourceModel model);
    }
}