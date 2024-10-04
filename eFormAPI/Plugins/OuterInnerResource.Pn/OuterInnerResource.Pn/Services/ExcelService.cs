/*
The MIT License (MIT)

Copyright (c) 2007 - 2019 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Infrastructure.Helpers;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Consts;
using OuterInnerResource.Pn.Infrastructure.Models.Report;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace OuterInnerResource.Pn.Services
{
    public class ExcelService : IExcelService
    {
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IOuterInnerResourceLocalizationService _outerInnerResourceLocalizationService;
        private readonly ILogger<ExcelService> _logger;

        public ExcelService(
            ILogger<ExcelService> logger,
            IOuterInnerResourceLocalizationService outerInnerResourceLocalizationService,
            IHttpContextAccessor httpAccessor)
        {
            _logger = logger;
            _outerInnerResourceLocalizationService = outerInnerResourceLocalizationService;
            _httpAccessor = httpAccessor;
        }

        #region Write to excel

        public bool WriteRecordsExportModelsToExcelFile(ReportModel reportModel,
            GenerateReportModel generateReportModel, string destFile)
        {
            // Create results directory if it doesn't exist
            Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "results"));

            var resultDocument = Path.Combine(Path.GetTempPath(), "results", $"report.xlsx");

            // Create the spreadsheet document
            using (SpreadsheetDocument spreadsheetDocument =
                   SpreadsheetDocument.Create(destFile, SpreadsheetDocumentType.Workbook))
            {
                // Create a workbook part
                WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                // Create a worksheet part
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Add Sheets to the workbook
                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

                // Append a new sheet and associate it with the workbook
                Sheet sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Report"
                };
                sheets.Append(sheet);

                // Get the sheet data to populate
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Add rows and cells using the OpenXML API
                AddTextCell(sheetData, ExcelConsts.EmployeeReport.PeriodFromTitleRow,
                    ExcelConsts.EmployeeReport.PeriodFromTitleCol,
                    _outerInnerResourceLocalizationService.GetString("DateFrom"));
                AddTextCell(sheetData, ExcelConsts.EmployeeReport.PeriodFromRow,
                    ExcelConsts.EmployeeReport.PeriodFromCol, generateReportModel.DateFrom.ToString());

                AddTextCell(sheetData, ExcelConsts.EmployeeReport.PeriodToTitleRow,
                    ExcelConsts.EmployeeReport.PeriodToTitleCol,
                    _outerInnerResourceLocalizationService.GetString("DateTo"));
                AddTextCell(sheetData, ExcelConsts.EmployeeReport.PeriodToRow, ExcelConsts.EmployeeReport.PeriodToCol,
                    generateReportModel.DateTo.ToString());

                AddTextCell(sheetData, ExcelConsts.EmployeeReport.PeriodTypeTitleRow,
                    ExcelConsts.EmployeeReport.PeriodTypeTitleCol,
                    _outerInnerResourceLocalizationService.GetString("ShowDataBy"));
                AddTextCell(sheetData, ExcelConsts.EmployeeReport.PeriodTypeRow,
                    ExcelConsts.EmployeeReport.PeriodTypeCol,
                    _outerInnerResourceLocalizationService.GetString(generateReportModel.Type.ToString()));

                AddTextCell(sheetData, ExcelConsts.EmployeeReport.ReportTitleRow,
                    ExcelConsts.EmployeeReport.ReportTitleCol,
                    _outerInnerResourceLocalizationService.GetString("Report"));
                AddTextCell(sheetData, ExcelConsts.EmployeeReport.ReportNameRow,
                    ExcelConsts.EmployeeReport.ReportNameCol,
                    _outerInnerResourceLocalizationService.GetString(reportModel.HumanReadableName));

                int entityPosition = 0;
                foreach (SubReportModel subReport in reportModel.SubReports)
                {
                    for (int i = 0; i < subReport.Entities.Count; i++)
                    {
                        ReportEntityModel reportEntity = subReport.Entities[i];
                        AddTextCell(sheetData, ExcelConsts.EmployeeReport.EntityNameStartRow + i + entityPosition,
                            ExcelConsts.EmployeeReport.EntityNameStartCol, reportEntity?.EntityName);
                        AddTextCell(sheetData,
                            ExcelConsts.EmployeeReport.RelatedEntityNameStartRow + i + entityPosition,
                            ExcelConsts.EmployeeReport.RelatedEntityNameStartCol, reportEntity?.RelatedEntityName);
                    }

                    for (int i = 0; i < reportModel.ReportHeaders.Count; i++)
                    {
                        ReportEntityHeaderModel reportHeader = reportModel.ReportHeaders[i];
                        AddTextCell(sheetData, ExcelConsts.EmployeeReport.HeaderStartRow + entityPosition,
                            ExcelConsts.EmployeeReport.HeaderStartCol + i, reportHeader?.HeaderValue);
                    }

                    for (int i = 0; i < subReport.Entities.Count; i++)
                    {
                        ReportEntityModel reportEntity = subReport.Entities[i];
                        AddDecimalCell(sheetData, ExcelConsts.EmployeeReport.VerticalSumStartRow + i + entityPosition,
                            ExcelConsts.EmployeeReport.VerticalSumStartCol, reportEntity?.TotalTime ?? 0);

                        for (int y = 0; y < reportEntity.TimePerTimeUnit.Count; y++)
                        {
                            AddDecimalCell(sheetData, ExcelConsts.EmployeeReport.DataStartRow + i + entityPosition,
                                ExcelConsts.EmployeeReport.DataStartCol + y, reportEntity.TimePerTimeUnit[y]);
                        }
                    }

                    AddTextCell(sheetData, ExcelConsts.EmployeeReport.VerticalSumTitleRow + entityPosition,
                        ExcelConsts.EmployeeReport.VerticalSumTitleCol, "Sum");

                    for (int i = 0; i < subReport.TotalTimePerTimeUnit.Count; i++)
                    {
                        AddDecimalCell(sheetData,
                            ExcelConsts.EmployeeReport.DataStartRow + subReport.Entities.Count + entityPosition,
                            ExcelConsts.EmployeeReport.HorizontalSumStartCol + i, subReport.TotalTimePerTimeUnit[i]);
                    }

                    AddDecimalCell(sheetData,
                        ExcelConsts.EmployeeReport.DataStartRow + subReport.Entities.Count + entityPosition,
                        ExcelConsts.EmployeeReport.TotalSumCol, subReport.TotalTime);
                    AddTextCell(sheetData,
                        ExcelConsts.EmployeeReport.DataStartRow + subReport.Entities.Count + entityPosition,
                        ExcelConsts.EmployeeReport.TotalSumTitleCol, "Sum");

                    entityPosition += subReport.Entities.Count + 3;
                }

                // Save the workbook
                workbookPart.Workbook.Save();
            }

            return true;
        }

        private void AddTextCell(SheetData sheetData, int rowIndex, int colIndex, string text)
        {
            Row row = GetOrCreateRow(sheetData, rowIndex);
            Cell cell = new Cell()
                { CellReference = GetCellReference(rowIndex, colIndex), DataType = CellValues.String };
            cell.CellValue = new CellValue(text);
            row.Append(cell);
        }

        private void AddDecimalCell(SheetData sheetData, int rowIndex, int colIndex, decimal value)
        {
            Row row = GetOrCreateRow(sheetData, rowIndex);
            Cell cell = new Cell()
                { CellReference = GetCellReference(rowIndex, colIndex), DataType = CellValues.Number };
            cell.CellValue = new CellValue(value.ToString());
            row.Append(cell);
        }

        private Row GetOrCreateRow(SheetData sheetData, int rowIndex)
        {
            Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            if (row == null)
            {
                row = new Row() { RowIndex = (uint)rowIndex };
                sheetData.Append(row);
            }

            return row;
        }

        private string GetCellReference(int rowIndex, int colIndex)
        {
            return $"{GetColumnName(colIndex)}{rowIndex}";
        }

        private string GetColumnName(int index)
        {
            int dividend = index;
            string columnName = String.Empty;
            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }

//         public bool WriteRecordsExportModelsToExcelFile(ReportModel reportModel, GenerateReportModel generateReportModel, string destFile)
//         {
//             Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "results"));
//
//             var resultDocument = Path.Combine(Path.GetTempPath(), "results",
//                 $"report.xlsx");
//
//             IXLWorkbook wb = new XLWorkbook();
//             var worksheet = wb.Worksheets.Add("Report");
//
//
//             // FileInfo file = new FileInfo(destFile);
//             // using (ExcelPackage package = new ExcelPackage(file))
//             // {
//                 // ExcelWorksheet worksheet = package.Workbook.Worksheets[ExcelConsts.MachineAreaReportSheetNumber];
//                 // Fill base info
//                 string periodFromTitle = _outerInnerResourceLocalizationService.GetString("DateFrom");
//                 worksheet.Cell(ExcelConsts.EmployeeReport.PeriodFromTitleRow, ExcelConsts.EmployeeReport.PeriodFromTitleCol).Value = periodFromTitle;
//                 worksheet.Cell(ExcelConsts.EmployeeReport.PeriodFromRow, ExcelConsts.EmployeeReport.PeriodFromCol).Value = generateReportModel.DateFrom;
//
//                 string periodToTitle = _outerInnerResourceLocalizationService.GetString("DateTo");
//                 worksheet.Cell(ExcelConsts.EmployeeReport.PeriodToTitleRow, ExcelConsts.EmployeeReport.PeriodToTitleCol).Value = periodToTitle;
//                 worksheet.Cell(ExcelConsts.EmployeeReport.PeriodToRow, ExcelConsts.EmployeeReport.PeriodToCol).Value = generateReportModel.DateTo;
//
//                 string showDataByTitle = _outerInnerResourceLocalizationService.GetString("ShowDataBy");
//                 worksheet.Cell(ExcelConsts.EmployeeReport.PeriodTypeTitleRow, ExcelConsts.EmployeeReport.PeriodTypeTitleCol).Value = showDataByTitle;
//                 string showDataByValue = _outerInnerResourceLocalizationService.GetString(generateReportModel.Type.ToString());
//                 worksheet.Cell(ExcelConsts.EmployeeReport.PeriodTypeRow, ExcelConsts.EmployeeReport.PeriodTypeCol).Value = showDataByValue;
//
//                 string reportTitle = _outerInnerResourceLocalizationService.GetString("Report");
//                 worksheet.Cell(ExcelConsts.EmployeeReport.ReportTitleRow, ExcelConsts.EmployeeReport.ReportTitleCol).Value = reportTitle;
//                 string reportName = _outerInnerResourceLocalizationService.GetString(reportModel.HumanReadableName);
//                 worksheet.Cell(ExcelConsts.EmployeeReport.ReportNameRow, ExcelConsts.EmployeeReport.ReportNameCol).Value = reportName;
//
// //                Debugger.Break();
//                 int entityPosition = 0;
//                 foreach (SubReportModel subReport in reportModel.SubReports)
//                 {
//                     // entity names
//                     for (int i = 0; i < subReport.Entities.Count; i++)
//                     {
//                         int rowIndex = ExcelConsts.EmployeeReport.EntityNameStartRow + i + entityPosition;
//                         ReportEntityModel reportEntity = subReport.Entities[i];
//                         worksheet.Cell(rowIndex, ExcelConsts.EmployeeReport.EntityNameStartCol).Value = reportEntity?.EntityName;
//                     }
//
//                     // related entity names
//                     for (int i = 0; i < subReport.Entities.Count; i++)
//                     {
//                         int rowIndex = ExcelConsts.EmployeeReport.RelatedEntityNameStartRow + i + entityPosition;
//                         ReportEntityModel reportEntity = subReport.Entities[i];
//                         worksheet.Cell(rowIndex, ExcelConsts.EmployeeReport.RelatedEntityNameStartCol).Value =
//                             reportEntity?.RelatedEntityName;
//                     }
//
//                     // headers
//                     for (int i = 0; i < reportModel.ReportHeaders.Count; i++)
//                     {
//                         ReportEntityHeaderModel reportHeader = reportModel.ReportHeaders[i];
//                         int colIndex = ExcelConsts.EmployeeReport.HeaderStartCol + i;
//                         int rowIndex = ExcelConsts.EmployeeReport.HeaderStartRow + entityPosition;
//                         worksheet.Cell(rowIndex, colIndex).Value = reportHeader?.HeaderValue;
//                         // , true, true, Color.Wheat)
//                     }
//
//                     // vertical sum
//                     for (int i = 0; i < subReport.Entities.Count; i++)
//                     {
//                         int rowIndex = ExcelConsts.EmployeeReport.VerticalSumStartRow + i + entityPosition;
//                         ReportEntityModel reportEntity = subReport.Entities[i];
//                         worksheet.Cell(rowIndex, ExcelConsts.EmployeeReport.VerticalSumStartCol).Value =
//                             reportEntity?.TotalTime;
//                         // , true, "0");
//                     }
//
//                     // vertical sum title
//                     worksheet.Cell(ExcelConsts.EmployeeReport.VerticalSumTitleRow + entityPosition, ExcelConsts.EmployeeReport.VerticalSumTitleCol).Value = "Sum";
//                         //, true, true);
//
//                     // data
//                     for (int i = 0; i < subReport.Entities.Count; i++)
//                     {
//                         ReportEntityModel reportEntity = subReport.Entities[i];
//                         int rowIndex = ExcelConsts.EmployeeReport.DataStartRow + i + entityPosition;
//                         for (int y = 0; y < reportEntity.TimePerTimeUnit.Count; y++)
//                         {
//                             decimal time = reportEntity.TimePerTimeUnit[y];
//                             int colIndex = ExcelConsts.EmployeeReport.DataStartCol + y;
//                             worksheet.Cell(rowIndex, colIndex).Value = time;
//                                 //, true, "0");
//                         }
//                     }
//
//                     // horizontal sum
//                     int horizontalSumRowIndex = ExcelConsts.EmployeeReport.DataStartRow + subReport.Entities.Count + entityPosition;
//                     for (int i = 0; i < subReport.TotalTimePerTimeUnit.Count; i++)
//                     {
//                         decimal time = subReport.TotalTimePerTimeUnit[i];
//                         int colIndex = ExcelConsts.EmployeeReport.HorizontalSumStartCol + i;
//                         worksheet.Cell(horizontalSumRowIndex, colIndex).Value =  time;
//                             // , true, "0");
//                     }
//
//                     // Report sum
//                     int totalSumRowIndex = ExcelConsts.EmployeeReport.DataStartRow + subReport.Entities.Count + entityPosition;
//                     decimal totalSum = subReport.TotalTime;
//                     worksheet.Cell(totalSumRowIndex, ExcelConsts.EmployeeReport.TotalSumCol).Value = totalSum;
//                         // , true);
//                     worksheet.Cell(totalSumRowIndex, ExcelConsts.EmployeeReport.TotalSumTitleCol).Value = "Sum";
//                         //, true);
//                     entityPosition += subReport.Entities.Count + 3;
//                 }
//
//                 wb.SaveAs(destFile); //Save the workbook.
//             // }
//             return true;
//         }

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

                Assembly assembly = typeof(EformOuterInnerResourcePlugin).GetTypeInfo().Assembly;
                Stream resourceStream = assembly.GetManifestResourceStream(
                    $"OuterInnerResource.Pn.Resources.Templates.{templateId}.xlsx");

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
                _logger.LogError(e, e.Message);
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