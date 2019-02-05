using MachineArea.Pn.Infrastructure.Models.Report;

namespace MachineArea.Pn.Abstractions
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
