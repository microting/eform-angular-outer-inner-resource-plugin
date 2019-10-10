using System.Collections.Generic;
using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;

namespace MachineArea.Pn.Controllers
{
    public class OuterInnerResourceSettingsController : Controller
    {
        private readonly IMachineAreaSettingsService _machineAreaSettingsService;

        public OuterInnerResourceSettingsController(IMachineAreaSettingsService machineAreaSettingsService)
        {
            _machineAreaSettingsService = machineAreaSettingsService;
        }

        [HttpGet]
        [Authorize(Roles = EformRole.Admin)]
        [Route("api/outer-inner-resource-pn/settings")]
        public async Task<OperationDataResult<MachineAreaBaseSettings>> GetSettings()
        {
            return await _machineAreaSettingsService.GetSettings();
        }


        [HttpPost]
        [Authorize(Roles = EformRole.Admin)]
        [Route("api/outer-inner-resource-pn/settings")]
        public async Task<OperationResult> UpdateSettings([FromBody] MachineAreaBaseSettings machineAreaSettingsModel)
        {
            return await _machineAreaSettingsService.UpdateSettings(machineAreaSettingsModel);
        }

        [HttpGet]
        [Authorize(Roles = EformRole.Admin)]
        [Route("api/outer-inner-resource-pn/settings/sites")]
        public async Task<OperationResult> GetSitesEnabled()
        {
            return await _machineAreaSettingsService.GetSitesEnabled();
        }

        [HttpPost]
        [Authorize(Roles = EformRole.Admin)]
        [Route("api/outer-inner-resource-pn/settings/sites")]
        public async Task<OperationResult> UpdateSitesEnabled(List<int> siteIds)
        {
            return await _machineAreaSettingsService.UpdateSitesEnabled(siteIds);
        }
    }
}
