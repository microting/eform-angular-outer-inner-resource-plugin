using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.OuterResources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Controllers
{
    [Authorize]
    public class OuterResourcesController : Controller
    {
        private readonly IAreaService _areaService;

        public OuterResourcesController(IAreaService areaService)
        {
            _areaService = areaService;
        }


        [HttpGet]
        [Route("api/outer-inner-resource-pn/areas")]
        public async Task<OperationDataResult<OuterResourcesModel>> GetAllAreas(OuterResourceRequestModel requestModel)
        {
            return await _areaService.GetAllAreas(requestModel);
        }

        [HttpGet]
        [Route("api/outer-inner-resource-pn/areas/{id}")]
        public async Task<OperationDataResult<OuterResourceModel>> GetSingleArea(int id)
        {
            return await _areaService.GetSingleArea(id);
        }

        [HttpPost]
        [Route("api/outer-inner-resource-pn/areas")]
        public async Task<OperationResult> CreateArea([FromBody] OuterResourceModel model)
        {
            return await _areaService.CreateArea(model);
        }

        [HttpPut]
        [Route("api/outer-inner-resource-pn/areas")]
        public async Task<OperationResult> UpdateArea([FromBody] OuterResourceModel model)
        {
            return await _areaService.UpdateArea(model);
        }

        [HttpDelete]
        [Route("api/outer-inner-resource-pn/areas/{id}")]
        public async Task<OperationResult> DeleteArea(int id)
        {
            return await _areaService.DeleteArea(id);
        }
    }
}