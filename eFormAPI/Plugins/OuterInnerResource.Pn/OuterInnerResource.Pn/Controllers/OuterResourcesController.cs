using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Models.OuterResources;

namespace OuterInnerResource.Pn.Controllers
{
    [Authorize]
    public class OuterResourcesController : Controller
    {
        private readonly IOuterResourceService _outerResourceService;

        public OuterResourcesController(IOuterResourceService outerResourceService)
        {
            _outerResourceService = outerResourceService;
        }


        [HttpGet]
        [Route("api/outer-inner-resource-pn/outer-resources")]
        public async Task<OperationDataResult<OuterResourcesModel>> GetAllAreas(OuterResourceRequestModel requestModel)
        {
            return await _outerResourceService.GetAllAreas(requestModel);
        }

        [HttpGet]
        [Route("api/outer-inner-resource-pn/outer-resources/{id}")]
        public async Task<OperationDataResult<OuterResourceModel>> GetSingleArea(int id)
        {
            return await _outerResourceService.GetSingleArea(id);
        }

        [HttpPost]
        [Route("api/outer-inner-resource-pn/outer-resources")]
        public async Task<OperationResult> CreateArea([FromBody] OuterResourceModel model)
        {
            return await _outerResourceService.CreateArea(model);
        }

        [HttpPut]
        [Route("api/outer-inner-resource-pn/outer-resources")]
        public async Task<OperationResult> UpdateArea([FromBody] OuterResourceModel model)
        {
            return await _outerResourceService.UpdateArea(model);
        }

        [HttpDelete]
        [Route("api/outer-inner-resource-pn/outer-resources/{id}")]
        public async Task<OperationResult> DeleteArea(int id)
        {
            return await _outerResourceService.DeleteArea(id);
        }
    }
}