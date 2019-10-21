using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Models.Settings;

namespace OuterInnerResource.Pn.Controllers
{
    public class OuterInnerResourceSettingsController : Controller
    {
        private readonly IOuterInnerResourceSettingsService _outerInnerResourceSettingsService;

        public OuterInnerResourceSettingsController(IOuterInnerResourceSettingsService outerInnerResourceSettingsService)
        {
            _outerInnerResourceSettingsService = outerInnerResourceSettingsService;
        }

        [HttpGet]
        [Authorize(Roles = EformRole.Admin)]
        [Route("api/outer-inner-resource-pn/settings")]
        public async Task<OperationDataResult<OuterInnerResourceSettings>> GetSettings()
        {
            return await _outerInnerResourceSettingsService.GetSettings();
        }


        [HttpPost]
        [Authorize(Roles = EformRole.Admin)]
        [Route("api/outer-inner-resource-pn/settings")]
        public async Task<OperationResult> UpdateSettings([FromBody] OuterInnerResourceSettings machineAreaSettingsModel)
        {
            return await _outerInnerResourceSettingsService.UpdateSettings(machineAreaSettingsModel);
        }

        [HttpGet]
        [Authorize(Roles = EformRole.Admin)]
        [Route("api/outer-inner-resource-pn/settings/sites")]
        public async Task<OperationResult> GetSitesEnabled()
        {
            return await _outerInnerResourceSettingsService.GetSitesEnabled();
        }

        [HttpPost]
        [Authorize(Roles = EformRole.Admin)]
        [Route("api/outer-inner-resource-pn/settings/sites")]
        public async Task<OperationResult> UpdateSitesEnabled(List<int> siteIds)
        {
            return await _outerInnerResourceSettingsService.UpdateSitesEnabled(siteIds);
        }
    }
}
