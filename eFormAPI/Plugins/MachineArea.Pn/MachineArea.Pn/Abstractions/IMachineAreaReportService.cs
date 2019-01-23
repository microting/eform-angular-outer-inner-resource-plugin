using MachineArea.Pn.Infrastructure.Models;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Abstractions
{
    public interface IMachineAreaReportService
    {
        OperationDataResult<ReportFullModel> GenerateReport(GenerateReportModel model);
        OperationResult GenerateReportFile(GenerateReportModel model);

    }
}
