using System.Threading.Tasks;
using MachineArea.Pn.Infrastructure.Models.Areas;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Abstractions
{
    public interface IAreaService
    {
        Task<OperationResult> CreateArea(AreaCreateModel model);
        Task<OperationResult> DeleteArea(int areaId);
        Task<OperationDataResult<AreasModel>> GetAllAreas(AreaRequestModel requestModel);
        Task<OperationDataResult<AreaModel>> GetSingleArea(int areaId);
        Task<OperationResult> UpdateArea(AreaUpdateModel model);
    }
}