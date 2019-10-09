using System.Threading.Tasks;
using MachineArea.Pn.Infrastructure.Models.InnerResources;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Abstractions
{
    public interface IMachineService
    {
        Task<OperationResult> CreateMachine(InnerResourceModel model);
        Task<OperationResult> DeleteMachine(int machineId);
        Task<OperationDataResult<InnerResourcesModel>> GetAllMachines(InnerResourceRequestModel requestModel);
        Task<OperationDataResult<InnerResourceModel>> GetSingleMachine(int machineId);
        Task<OperationResult> UpdateMachine(InnerResourceModel model);
    }
}