using System.Threading.Tasks;
using MachineArea.Pn.Infrastructure.Models.Machines;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Abstractions
{
    public interface IMachineService
    {
        Task<OperationResult> CreateMachine(MachineCreateModel model);
        Task<OperationResult> DeleteMachine(int machineId);
        Task<OperationDataResult<MachinesModel>> GetAllMachines(MachineRequestModel requestModel);
        Task<OperationDataResult<MachineModel>> GetSingleMachine(int machineId);
        Task<OperationResult> UpdateMachine(MachineUpdateModel model);
    }
}