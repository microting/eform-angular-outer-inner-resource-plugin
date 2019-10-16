using System.Collections.Generic;
using System.Threading.Tasks;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using OuterInnerResource.Pn.Infrastructure.Models.Settings;

namespace OuterInnerResource.Pn.Abstractions
{
    public interface IOuterInnerResourceSettingsService
    {
        Task<OperationDataResult<MachineAreaBaseSettings>> GetSettings();
        Task<OperationResult> UpdateSettings(MachineAreaBaseSettings machineAreaSettingsModel);
        Task<OperationDataResult<List<int>>> GetSitesEnabled();
        Task<OperationResult> UpdateSitesEnabled(List<int> siteIds);
    }
}
