using System.Threading.Tasks;
using MachineArea.Pn.Infrastructure.Models;
using MachineArea.Pn.Infrastructure.Models.Report;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Abstractions
{
    public interface IMachineAreaReportService
    {
        Task<OperationDataResult<ReportModel>> GenerateReport(GenerateReportModel model);
        Task<OperationDataResult<ReportNamesModel>> GetReportNames();
        Task<OperationDataResult<FileStreamModel>> GenerateReportFile(GenerateReportModel model);

    }
}
