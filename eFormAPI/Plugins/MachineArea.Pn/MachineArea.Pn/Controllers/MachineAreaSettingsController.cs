using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;

namespace MachineArea.Pn.Controllers
{
    public class MachineAreaSettingsController : Controller
    {
        private readonly IMachineAreaSettingsService _machineAreaSettingsService;

        public MachineAreaSettingsController(IMachineAreaSettingsService machineAreaSettingsService)
        {
            _machineAreaSettingsService = machineAreaSettingsService;
        }

        [HttpGet]
        [Authorize(Roles = EformRole.Admin)]
        [Route("api/machine-area-pn/settings")]
        public OperationDataResult<MachineAreaBaseSettings> GetSettings()
        {
            return _machineAreaSettingsService.GetSettings();
        }


        [HttpPost]
        [Authorize(Roles = EformRole.Admin)]
        [Route("api/machine-area-pn/settings")]
        public async Task<OperationResult> UpdateSettings([FromBody] MachineAreaBaseSettings machineAreaSettingsModel)
        {
            return await _machineAreaSettingsService.UpdateSettings(machineAreaSettingsModel);
        }
    }
}
