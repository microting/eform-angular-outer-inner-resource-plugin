using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.Areas;
using MachineArea.Pn.Infrastructure.Models.Machines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Controllers
{
    [Authorize]
    public class MachinesController : Controller
    {
        private readonly IMachineService _machineService;

        public MachinesController(IMachineService machineService)
        {
            _machineService = machineService;
        }

        [HttpGet]
        [Route("api/machine-area-pn/machines")]
        public async Task<OperationDataResult<MachinesModel>> GetAllMachines(MachineRequestModel requestModel)
        {
            return await _machineService.GetAllMachines(requestModel);
        }

        [HttpGet]
        [Route("api/machine-area-pn/machines/{id}")]
        public async Task<OperationDataResult<MachineModel>> GetSingleMachine(int id)
        {
            return await _machineService.GetSingleMachine(id);
        }

        [HttpPost]
        [Route("api/machine-area-pn/machines")]
        public async Task<OperationResult> CreateMachine([FromBody] MachineCreateModel model)
        {
            return await _machineService.CreateMachine(model);
        }

        [HttpPut]
        [Route("api/machine-area-pn/machines")]
        public async Task<OperationResult> UpdateMachines([FromBody] MachineUpdateModel model)
        {
            return await _machineService.UpdateMachine(model);
        }

        [HttpDelete]
        [Route("api/machine-area-pn/machines/{id}")]
        public async Task<OperationResult> DeleteArea(int id)
        {
            return await _machineService.DeleteMachine(id);
        }
    }
}