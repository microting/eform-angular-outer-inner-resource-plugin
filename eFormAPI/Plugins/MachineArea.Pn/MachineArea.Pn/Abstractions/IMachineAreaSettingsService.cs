using MachineArea.Pn.Infrastructure.Models;
using MachineArea.Pn.Infrastructure.Models.Settings;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Abstractions
{
    public interface IMachineAreaSettingsService
    {
        OperationDataResult<MachineAreaBaseSettings> GetSettings();
        OperationResult UpdateSettings(MachineAreaBaseSettings machineAreaSettingsModel);
    }
}
