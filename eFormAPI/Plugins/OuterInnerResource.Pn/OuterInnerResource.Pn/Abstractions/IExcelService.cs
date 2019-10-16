using OuterInnerResource.Pn.Infrastructure.Models.Report;

namespace OuterInnerResource.Pn.Abstractions
{
    public interface IExcelService
    {
        bool WriteRecordsExportModelsToExcelFile(
            ReportModel reportModel,
            GenerateReportModel generateReportModel,
            string destFile);
        string CopyTemplateForNewAccount(string templateName);
    }
}
