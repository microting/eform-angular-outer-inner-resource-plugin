using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models;
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
//        [Authorize(Roles = EformRole.Admin)]
        [Route("api/machine-area-pn/settings")]
        public OperationDataResult<MachineAreaSettingsModel> GetSettings()
        {
            return _machineAreaSettingsService.GetSettings();
        }


        [HttpPost]
//        [Authorize(Roles = EformRole.Admin)]
        [Route("api/machine-area-pn/settings")]
        public OperationResult UpdateSettings(MachineAreaSettingsModel machineAreaSettingsModel)
        {
            return _machineAreaSettingsService.UpdateSettings(machineAreaSettingsModel);
        }
    }
}
