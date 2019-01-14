using MachineArea.Pn.Infrastructure.Models;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Abstractions
{
    public interface IMachineAreaSettingsService
    {
        OperationDataResult<MachineAreaSettingsModel> GetSettings();
        OperationResult UpdateSettings(MachineAreaSettingsModel machineAreaSettingsModel);
    }
}
