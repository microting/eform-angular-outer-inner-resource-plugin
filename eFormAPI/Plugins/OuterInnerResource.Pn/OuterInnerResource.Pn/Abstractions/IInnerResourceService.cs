using System.Threading.Tasks;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using OuterInnerResource.Pn.Infrastructure.Models.InnerResources;

namespace OuterInnerResource.Pn.Abstractions
{
    public interface IInnerResourceService
    {
        Task<OperationResult> Create(InnerResourceModel model);
        Task<OperationResult> Delete(int innerResourceId);
        Task<OperationDataResult<InnerResourcesModel>> Index(InnerResourceRequestModel requestModel);
        Task<OperationDataResult<InnerResourceModel>> Get(int innerResourceId);
        Task<OperationResult> Update(InnerResourceModel model);
    }
}