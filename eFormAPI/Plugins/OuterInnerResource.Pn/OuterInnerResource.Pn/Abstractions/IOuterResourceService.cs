using System.Threading.Tasks;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using OuterInnerResource.Pn.Infrastructure.Models.OuterResources;

namespace OuterInnerResource.Pn.Abstractions
{
    public interface IOuterResourceService
    {
        Task<OperationResult> Create(OuterResourceModel model);
        Task<OperationResult> Delete(int outerResourceId);
        Task<OperationDataResult<OuterResourcesModel>> Index(OuterResourceRequestModel requestModel);
        Task<OperationDataResult<OuterResourceModel>> Get(int areaId);
        Task<OperationResult> Update(OuterResourceModel model);
    }
}