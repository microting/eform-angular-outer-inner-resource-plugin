using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Consts;
using MachineArea.Pn.Infrastructure.Extensions;
using MachineArea.Pn.Infrastructure.Models.Report;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Infrastructure.Helpers;
using OfficeOpenXml;

namespace MachineArea.Pn.Services
{
    public class ExcelService : IExcelService
    {
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IMachineAreaLocalizationService _machineAreaLocalizationService;
        private readonly ILogger<ExcelService> _logger;

        public ExcelService(
            ILogger<ExcelService> logger,
            IMachineAreaLocalizationService machineAreaLocalizationService,
            IHttpContextAccessor httpAccessor)
        {
            _logger = logger;
            _machineAreaLocalizationService = machineAreaLocalizationService;
            _httpAccessor = httpAccessor;
        }

        #region Write to excel

        public bool WriteRecordsExportModelsToExcelFile(ReportModel reportModel, GenerateReportModel generateReportModel, string destFile)
        {
            var file = new FileInfo(destFile);
            using (var package = new ExcelPackage(file))
            {
                var worksheet = package.Workbook.Worksheets[ExcelConsts.MachineAreaReportSheetNumber];
                // Fill base info
                var periodFromTitle = _machineAreaLocalizationService.GetString("DateFrom");
                worksheet.Cells[ExcelConsts.EmployeeReport.PeriodFromTitleRow, ExcelConsts.EmployeeReport.PeriodFromTitleCol].Value = periodFromTitle;
                worksheet.Cells[ExcelConsts.EmployeeReport.PeriodFromRow, ExcelConsts.EmployeeReport.PeriodFromCol].Value = generateReportModel.DateFrom;

                var periodToTitle = _machineAreaLocalizationService.GetString("DateTo");
                worksheet.Cells[ExcelConsts.EmployeeReport.PeriodToTitleRow, ExcelConsts.EmployeeReport.PeriodToTitleCol].Value = periodToTitle;
                worksheet.Cells[ExcelConsts.EmployeeReport.PeriodToRow, ExcelConsts.EmployeeReport.PeriodToCol].Value = generateReportModel.DateTo;        

                var showDataByTitle = _machineAreaLocalizationService.GetString("ShowDataBy");
                worksheet.Cells[ExcelConsts.EmployeeReport.PeriodTypeTitleRow, ExcelConsts.EmployeeReport.PeriodTypeTitleCol].Value = showDataByTitle;        
                var showDataByValue = _machineAreaLocalizationService.GetString(generateReportModel.Type.ToString());
                worksheet.Cells[ExcelConsts.EmployeeReport.PeriodTypeRow, ExcelConsts.EmployeeReport.PeriodTypeCol].Value = showDataByValue;        
                
                var reportTitle = _machineAreaLocalizationService.GetString("Report");
                worksheet.Cells[ExcelConsts.EmployeeReport.ReportTitleRow, ExcelConsts.EmployeeReport.ReportTitleCol].Value = reportTitle;
                var reportName = _machineAreaLocalizationService.GetString(reportModel.Relationship.ToString());
                worksheet.Cells[ExcelConsts.EmployeeReport.ReportNameRow, ExcelConsts.EmployeeReport.ReportNameCol].Value = reportName;        

                // entity names
                for (var i = 0; i < reportModel.Entities.Count; i++)
                {
                    var rowIndex = ExcelConsts.EmployeeReport.EntityNameStartRow + i;
                    var reportEntity = reportModel.Entities[i];
                    worksheet.UpdateValue(rowIndex, ExcelConsts.EmployeeReport.EntityNameStartCol, reportEntity?.EntityName, true);
                }

                // related entity names
                for (var i = 0; i < reportModel.Entities.Count; i++)
                {
                    var rowIndex = ExcelConsts.EmployeeReport.RelatedEntityNameStartRow + i;
                    var reportEntity = reportModel.Entities[i];
                    worksheet.UpdateValue(rowIndex, ExcelConsts.EmployeeReport.RelatedEntityNameStartCol,
                        reportEntity?.RelatedEntityName, true);
                }

                // headers
                for (var i = 0; i < reportModel.ReportHeaders.Count; i++)
                {
                    var reportHeader = reportModel.ReportHeaders[i];
                    var colIndex = ExcelConsts.EmployeeReport.HeaderStartCol + i;
                    worksheet.UpdateValue(ExcelConsts.EmployeeReport.HeaderStartRow, colIndex, reportHeader?.HeaderValue, true, true, Color.Wheat);
                }

                // vertical sum
                for (var i = 0; i < reportModel.Entities.Count; i++)
                {
                    var rowIndex = ExcelConsts.EmployeeReport.VerticalSumStartRow + i;
                    var reportEntity = reportModel.Entities[i];
                    worksheet.UpdateValue(rowIndex, ExcelConsts.EmployeeReport.VerticalSumStartCol, reportEntity?.TotalTime, true, "0");
                }

                // vertical sum title
                worksheet.UpdateValue(ExcelConsts.EmployeeReport.VerticalSumTitleRow, ExcelConsts.EmployeeReport.VerticalSumTitleCol, "Sum", true);

                // data
                for (var i = 0; i < reportModel.Entities.Count; i++)
                {
                    var reportEntity = reportModel.Entities[i];
                    var rowIndex = ExcelConsts.EmployeeReport.DataStartRow + i;
                    for (var y = 0; y < reportEntity.TimePerTimeUnit.Count; y++)
                    {
                        var time = reportEntity.TimePerTimeUnit[y];
                        var colIndex = ExcelConsts.EmployeeReport.DataStartCol + y;
                        worksheet.UpdateValue(rowIndex, colIndex, time, true, "0");
                    }
                }

                // horizontal sum
                var horizontalSumRowIndex = ExcelConsts.EmployeeReport.DataStartRow + reportModel.Entities.Count;
                for (var i = 0; i < reportModel.TotalTimePerTimeUnit.Count; i++)
                {
                    var time = reportModel.TotalTimePerTimeUnit[i];
                    var colIndex = ExcelConsts.EmployeeReport.HorizontalSumStartCol + i;
                    worksheet.UpdateValue(horizontalSumRowIndex, colIndex, time, true, "0");
                }
                
                // Report sum
                var totalSumRowIndex = ExcelConsts.EmployeeReport.DataStartRow + reportModel.Entities.Count;
                var totalSum = reportModel.TotalTime;
                worksheet.UpdateValue(totalSumRowIndex, ExcelConsts.EmployeeReport.TotalSumCol, totalSum, true);
                worksheet.UpdateValue(totalSumRowIndex, ExcelConsts.EmployeeReport.TotalSumTitleCol, "Sum", true);
                
                package.Save(); //Save the workbook.
            }
            return true;
        }

        #endregion

        #region Working with file system
        
        private int UserId
        {
            get
            {
                var value = _httpAccessor?.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return value == null ? 0 : int.Parse(value);
            }
        }

        private static string GetExcelStoragePath()
        {
            var path = Path.Combine(PathHelper.GetStoragePath(), "excel-storage");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Will return filename for excel file
        /// </summary>
        /// <returns></returns>
        private static string BuildFileNameForExcelFile(int userId, string templateId)
        {
            return $"{templateId}-{userId}-{DateTime.UtcNow.Ticks}.xlsx";
        }

        /// <summary>
        /// Get path and filename for particular user
        /// </summary>
        /// <returns></returns>
        private static string GetFilePathForUser(int userId, string templateId)
        {
            var filesDir = GetExcelStoragePath();
            var destFile = Path.Combine(filesDir, BuildFileNameForExcelFile(userId, templateId));
            return destFile;
        }

        /// <summary>
        /// Copy template file to new excel file
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">userId</exception>
        public string CopyTemplateForNewAccount(string templateId)
        {
            string destFile = null;
            try
            {
                var userId = UserId;
                if (userId <= 0)
                {
                    throw new ArgumentNullException(nameof(userId));
                }

                var assembly = typeof(EformMachineAreaPlugin).GetTypeInfo().Assembly;
                var resourceStream = assembly.GetManifestResourceStream(
                    $"MachineArea.Pn.Resources.Templates.{templateId}.xlsx");

                destFile = GetFilePathForUser(userId, templateId);
                if (File.Exists(destFile))
                {
                    File.Delete(destFile);
                }
                using (var fileStream = File.Create(destFile))
                {
                    resourceStream.Seek(0, SeekOrigin.Begin);
                    resourceStream.CopyTo(fileStream);
                }
                return destFile;
            }
            catch (Exception e)
            {
                _logger.LogError(e,e.Message);
                if (File.Exists(destFile))
                {
                    File.Delete(destFile);
                }
                return null;
            }
        }

        #endregion
    }
}