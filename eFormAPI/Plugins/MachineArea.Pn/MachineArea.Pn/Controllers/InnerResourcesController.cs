using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.InnerResources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Controllers
{
    [Authorize]
    public class InnerResourcesController : Controller
    {
        private readonly IMachineService _machineService;

        public InnerResourcesController(IMachineService machineService)
        {
            _machineService = machineService;
        }

        [HttpGet]
        [Route("api/outer-inner-resource-pn/inner-resources")]
        public async Task<OperationDataResult<InnerResourcesModel>> GetAllMachines(InnerResourceRequestModel requestModel)
        {
            return await _machineService.GetAllMachines(requestModel);
        }

        [HttpGet]
        [Route("api/outer-inner-resource-pn/inner-resources/{id}")]
        public async Task<OperationDataResult<InnerResourceModel>> GetSingleMachine(int id)
        {
            return await _machineService.GetSingleMachine(id);
        }

        [HttpPost]
        [Route("api/outer-inner-resource-pn/inner-resources")]
        public async Task<OperationResult> CreateMachine([FromBody] InnerResourceModel model)
        {
            return await _machineService.CreateMachine(model);
        }

        [HttpPut]
        [Route("api/outer-inner-resource-pn/inner-resources")]
        public async Task<OperationResult> UpdateMachines([FromBody] InnerResourceModel model)
        {
            return await _machineService.UpdateMachine(model);
        }

        [HttpDelete]
        [Route("api/outer-inner-resource-pn/inner-resources/{id}")]
        public async Task<OperationResult> DeleteArea(int id)
        {
            return await _machineService.DeleteMachine(id);
        }
    }
}