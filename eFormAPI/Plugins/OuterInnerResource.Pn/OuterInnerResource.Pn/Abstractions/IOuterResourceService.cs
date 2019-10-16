using System.Threading.Tasks;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using OuterInnerResource.Pn.Infrastructure.Models.OuterResources;

namespace OuterInnerResource.Pn.Abstractions
{
    public interface IOuterResourceService
    {
        Task<OperationResult> CreateArea(OuterResourceModel model);
        Task<OperationResult> DeleteArea(int areaId);
        Task<OperationDataResult<OuterResourcesModel>> GetAllAreas(OuterResourceRequestModel requestModel);
        Task<OperationDataResult<OuterResourceModel>> GetSingleArea(int areaId);
        Task<OperationResult> UpdateArea(OuterResourceModel model);
    }
}