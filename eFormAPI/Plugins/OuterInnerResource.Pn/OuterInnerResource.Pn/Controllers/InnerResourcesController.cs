using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Models.InnerResources;

namespace OuterInnerResource.Pn.Controllers
{
    [Authorize]
    public class InnerResourcesController : Controller
    {
        private readonly IInnerResourceService _innerResourceService;

        public InnerResourcesController(IInnerResourceService innerResourceService)
        {
            _innerResourceService = innerResourceService;
        }

        [HttpGet]
        [Route("api/outer-inner-resource-pn/inner-resources")]
        public async Task<OperationDataResult<InnerResourcesModel>> GetAllMachines(InnerResourceRequestModel requestModel)
        {
            return await _innerResourceService.GetAllMachines(requestModel);
        }

        [HttpGet]
        [Route("api/outer-inner-resource-pn/inner-resources/{id}")]
        public async Task<OperationDataResult<InnerResourceModel>> GetSingleMachine(int id)
        {
            return await _innerResourceService.GetSingleMachine(id);
        }

        [HttpPost]
        [Route("api/outer-inner-resource-pn/inner-resources")]
        [Authorize(Policy = OuterInnerResourceClaims.CreateMachines)]
        public async Task<OperationResult> CreateMachine([FromBody] InnerResourceModel model)
        {
            return await _innerResourceService.CreateMachine(model);
        }

        [HttpPut]
        [Route("api/outer-inner-resource-pn/inner-resources")]
        public async Task<OperationResult> UpdateMachines([FromBody] InnerResourceModel model)
        {
            return await _innerResourceService.UpdateMachine(model);
        }

        [HttpDelete]
        [Route("api/outer-inner-resource-pn/inner-resources/{id}")]
        public async Task<OperationResult> DeleteArea(int id)
        {
            return await _innerResourceService.DeleteMachine(id);
        }
    }
}