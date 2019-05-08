using System;
using System.Diagnostics;
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
            FileInfo file = new FileInfo(destFile);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[ExcelConsts.MachineAreaReportSheetNumber];
                // Fill base info
                string periodFromTitle = _machineAreaLocalizationService.GetString("DateFrom");
                worksheet.Cells[ExcelConsts.EmployeeReport.PeriodFromTitleRow, ExcelConsts.EmployeeReport.PeriodFromTitleCol].Value = periodFromTitle;
                worksheet.Cells[ExcelConsts.EmployeeReport.PeriodFromRow, ExcelConsts.EmployeeReport.PeriodFromCol].Value = generateReportModel.DateFrom;

                string periodToTitle = _machineAreaLocalizationService.GetString("DateTo");
                worksheet.Cells[ExcelConsts.EmployeeReport.PeriodToTitleRow, ExcelConsts.EmployeeReport.PeriodToTitleCol].Value = periodToTitle;
                worksheet.Cells[ExcelConsts.EmployeeReport.PeriodToRow, ExcelConsts.EmployeeReport.PeriodToCol].Value = generateReportModel.DateTo;        

                string showDataByTitle = _machineAreaLocalizationService.GetString("ShowDataBy");
                worksheet.Cells[ExcelConsts.EmployeeReport.PeriodTypeTitleRow, ExcelConsts.EmployeeReport.PeriodTypeTitleCol].Value = showDataByTitle;        
                string showDataByValue = _machineAreaLocalizationService.GetString(generateReportModel.Type.ToString());
                worksheet.Cells[ExcelConsts.EmployeeReport.PeriodTypeRow, ExcelConsts.EmployeeReport.PeriodTypeCol].Value = showDataByValue;        
                
                string reportTitle = _machineAreaLocalizationService.GetString("Report");
                worksheet.Cells[ExcelConsts.EmployeeReport.ReportTitleRow, ExcelConsts.EmployeeReport.ReportTitleCol].Value = reportTitle;
                string reportName = _machineAreaLocalizationService.GetString(reportModel.Relationship.ToString());
                worksheet.Cells[ExcelConsts.EmployeeReport.ReportNameRow, ExcelConsts.EmployeeReport.ReportNameCol].Value = reportName;

                Debugger.Break();
                int entityPosition = 0;
                foreach (SubReportModel subReport in reportModel.SubReports)
                {
                    // entity names
                    for (int i = 0; i < subReport.Entities.Count; i++)
                    {
                        int rowIndex = ExcelConsts.EmployeeReport.EntityNameStartRow + i + entityPosition;
                        ReportEntityModel reportEntity = subReport.Entities[i];
                        worksheet.UpdateValue(rowIndex, ExcelConsts.EmployeeReport.EntityNameStartCol, reportEntity?.EntityName, true);
                    }

                    // related entity names
                    for (int i = 0; i < subReport.Entities.Count; i++)
                    {
                        int rowIndex = ExcelConsts.EmployeeReport.RelatedEntityNameStartRow + i + entityPosition;
                        ReportEntityModel reportEntity = subReport.Entities[i];
                        worksheet.UpdateValue(rowIndex, ExcelConsts.EmployeeReport.RelatedEntityNameStartCol,
                            reportEntity?.RelatedEntityName, true);
                    }

                    // headers
                    for (int i = 0; i < reportModel.ReportHeaders.Count; i++)
                    {
                        ReportEntityHeaderModel reportHeader = reportModel.ReportHeaders[i];
                        int colIndex = ExcelConsts.EmployeeReport.HeaderStartCol + i;
                        int rowIndex = ExcelConsts.EmployeeReport.HeaderStartRow + entityPosition;
                        worksheet.UpdateValue(rowIndex, colIndex, reportHeader?.HeaderValue, true, true, Color.Wheat);
                    }

                    // vertical sum
                    for (int i = 0; i < subReport.Entities.Count; i++)
                    {
                        int rowIndex = ExcelConsts.EmployeeReport.VerticalSumStartRow + i + entityPosition;
                        ReportEntityModel reportEntity = subReport.Entities[i];
                        worksheet.UpdateValue(rowIndex, ExcelConsts.EmployeeReport.VerticalSumStartCol, reportEntity?.TotalTime, true, "0");
                    }

                    // vertical sum title
                    worksheet.UpdateValue(ExcelConsts.EmployeeReport.VerticalSumTitleRow + entityPosition, ExcelConsts.EmployeeReport.VerticalSumTitleCol, "Sum", true, true);

                    // data
                    for (int i = 0; i < subReport.Entities.Count; i++)
                    {
                        ReportEntityModel reportEntity = subReport.Entities[i];
                        int rowIndex = ExcelConsts.EmployeeReport.DataStartRow + i + entityPosition;
                        for (int y = 0; y < reportEntity.TimePerTimeUnit.Count; y++)
                        {
                            decimal time = reportEntity.TimePerTimeUnit[y];
                            int colIndex = ExcelConsts.EmployeeReport.DataStartCol + y;
                            worksheet.UpdateValue(rowIndex, colIndex, time, true, "0");
                        }
                    }

                    // horizontal sum
                    int horizontalSumRowIndex = ExcelConsts.EmployeeReport.DataStartRow + subReport.Entities.Count + entityPosition;
                    for (int i = 0; i < subReport.TotalTimePerTimeUnit.Count; i++)
                    {
                        decimal time = subReport.TotalTimePerTimeUnit[i];
                        int colIndex = ExcelConsts.EmployeeReport.HorizontalSumStartCol + i;
                        worksheet.UpdateValue(horizontalSumRowIndex, colIndex, time, true, "0");
                    }

                    // Report sum
                    int totalSumRowIndex = ExcelConsts.EmployeeReport.DataStartRow + subReport.Entities.Count + entityPosition;
                    decimal totalSum = subReport.TotalTime;
                    worksheet.UpdateValue(totalSumRowIndex, ExcelConsts.EmployeeReport.TotalSumCol, totalSum, true);
                    worksheet.UpdateValue(totalSumRowIndex, ExcelConsts.EmployeeReport.TotalSumTitleCol, "Sum", true);
                    entityPosition += subReport.Entities.Count + 3;
                } 

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
                string value = _httpAccessor?.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return value == null ? 0 : int.Parse(value);
            }
        }

        private static string GetExcelStoragePath()
        {
            string path = Path.Combine(PathHelper.GetStoragePath(), "excel-storage");
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
            string filesDir = GetExcelStoragePath();
            string destFile = Path.Combine(filesDir, BuildFileNameForExcelFile(userId, templateId));
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
                int userId = UserId;
                if (userId <= 0)
                {
                    throw new ArgumentNullException(nameof(userId));
                }

                Assembly assembly = typeof(EformMachineAreaPlugin).GetTypeInfo().Assembly;
                Stream resourceStream = assembly.GetManifestResourceStream(
                    $"MachineArea.Pn.Resources.Templates.{templateId}.xlsx");

                destFile = GetFilePathForUser(userId, templateId);
                if (File.Exists(destFile))
                {
                    File.Delete(destFile);
                }
                using (FileStream fileStream = File.Create(destFile))
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