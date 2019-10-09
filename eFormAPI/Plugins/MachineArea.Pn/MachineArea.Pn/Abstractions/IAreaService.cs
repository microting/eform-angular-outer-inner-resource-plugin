using System.Threading.Tasks;
using MachineArea.Pn.Infrastructure.Models.OuterResources;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Abstractions
{
    public interface IAreaService
    {
        Task<OperationResult> CreateArea(OuterResourceModel model);
        Task<OperationResult> DeleteArea(int areaId);
        Task<OperationDataResult<OuterResourcesModel>> GetAllAreas(OuterResourceRequestModel requestModel);
        Task<OperationDataResult<OuterResourceModel>> GetSingleArea(int areaId);
        Task<OperationResult> UpdateArea(OuterResourceModel model);
    }
}