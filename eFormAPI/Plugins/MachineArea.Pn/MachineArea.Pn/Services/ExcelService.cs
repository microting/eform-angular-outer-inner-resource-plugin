using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Consts;
using MachineArea.Pn.Infrastructure.Models.Report;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Infrastructure.Helpers;
using OfficeOpenXml;

namespace MachineArea.Pn.Services
{
    public class ExcelService : IExcelService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IMachineAreaLocalizationService _machineAreaLocalizationService;
        private readonly ILogger<ExcelService> _logger;

        public ExcelService(
            IHostingEnvironment hostingEnvironment,
            ILogger<ExcelService> logger,
            IMachineAreaLocalizationService machineAreaLocalizationService,
            IHttpContextAccessor httpAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
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
                worksheet.Cells[ExcelConsts.PeriodFromTitleRow, ExcelConsts.PeriodFromTitleCol].Value = periodFromTitle;
                var periodFromDate = generateReportModel.DateFrom.ToString(CultureInfo.CurrentCulture);
                worksheet.Cells[ExcelConsts.PeriodFromRow, ExcelConsts.PeriodFromCol].Value = periodFromDate;
                //
                var periodToTitle = _machineAreaLocalizationService.GetString("DateTo");
                worksheet.Cells[ExcelConsts.PeriodToTitleRow, ExcelConsts.PeriodToTitleCol].Value = periodToTitle;
                var periodToDate = generateReportModel.DateFrom.ToString(CultureInfo.CurrentCulture);
                worksheet.Cells[ExcelConsts.PeriodToRow, ExcelConsts.PeriodToCol].Value = periodToDate;        

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
        private static string BuildFileNameForExcelFile(int userId)
        {
            return $"machinearea-report-{userId}-{DateTime.UtcNow.Ticks}.xlsx";
        }

        /// <summary>
        /// Get path and filename for particular user
        /// </summary>
        /// <returns></returns>
        private static string GetFilePathForUser(int userId)
        {
            var filesDir = GetExcelStoragePath();
            var destFile = Path.Combine(filesDir, BuildFileNameForExcelFile(userId));
            return destFile;
        }

        /// <summary>
        /// Copy template file to new excel file
        /// </summary>
        /// <param name="templateName">Name of the template.</param>
        /// <returns></returns>
        public string CopyTemplateForNewAccount(string templateName)
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
                    $"MachineArea.Pn.Resources.Templates.{templateName}");

                destFile = GetFilePathForUser(userId);
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