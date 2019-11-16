using System.Threading.Tasks;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using OuterInnerResource.Pn.Infrastructure.Models;
using OuterInnerResource.Pn.Infrastructure.Models.Report;

namespace OuterInnerResource.Pn.Abstractions
{
    public interface IOuterInnerResourceReportService
    {
        Task<OperationDataResult<ReportModel>> GenerateReport(GenerateReportModel model);
        OperationDataResult<ReportNamesModel> GetReportNames();
        Task<OperationDataResult<FileStreamModel>> GenerateReportFile(GenerateReportModel model);

    }
}
