using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eFormCore;
using eFormShared;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Enums;
using MachineArea.Pn.Infrastructure.Extensions;
using MachineArea.Pn.Infrastructure.Helpers;
using MachineArea.Pn.Infrastructure.Models;
using MachineArea.Pn.Infrastructure.Models.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

namespace MachineArea.Pn.Services
{
    public class MachineAreaReportService : IMachineAreaReportService
    {
        private readonly ILogger<MachineAreaReportService> _logger;
        private readonly IMachineAreaLocalizationService _machineAreaLocalizationService;
        private readonly IExcelService _excelService;
        private readonly MachineAreaPnDbContext _dbContext;
        private readonly IEFormCoreService _coreHelper;

        public MachineAreaReportService(ILogger<MachineAreaReportService> logger,
            MachineAreaPnDbContext dbContext,
            IEFormCoreService coreHelper,
            IMachineAreaLocalizationService machineAreaLocalizationService,
            IExcelService excelService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _coreHelper = coreHelper;
            _machineAreaLocalizationService = machineAreaLocalizationService;
            _excelService = excelService;
        }

        public async Task<OperationDataResult<ReportModel>> GenerateReport(GenerateReportModel model)
        {
            try
            {
                Core core = _coreHelper.GetCore();
                List<Site_Dto> sitesList = core.SiteReadAll(false);
                DateTime modelDateFrom = new DateTime(
                    model.DateFrom.Year,
                    model.DateFrom.Month,
                    model.DateFrom.Day,
                    0,0,0);

                DateTime modelDateTo = new DateTime(
                    model.DateTo.Year,
                    model.DateTo.Month,
                    model.DateTo.Day,
                    23,59,59);

                List<MachineAreaTimeRegistration> jobsList = await _dbContext.MachineAreaTimeRegistrations
                    .Include(x => x.Machine)
                    .Include(x => x.Area)
                    .Where(x => x.DoneAt >= modelDateFrom && x.DoneAt <= modelDateTo)
                    .ToListAsync();
                List<ReportEntityModel> reportEntitiesList = new List<ReportEntityModel>();
                List<DateTime> reportDates = new List<DateTime>();
                List<ReportEntityHeaderModel> reportHeaders = new List<ReportEntityHeaderModel>();
                ReportModel finalModel = new ReportModel();
                switch (model.Type)
                {
                    case ReportType.Day:
                        DateTime dateFrom = new DateTime(
                            model.DateFrom.Year,
                            model.DateFrom.Month,
                            model.DateFrom.Day,
                            0,0,0);

                        DateTime dateTo = new DateTime(
                            model.DateTo.Year,
                            model.DateTo.Month,
                            model.DateTo.Day,
                            23,59,59);

                        for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
                        {
                            reportDates.Add(date);
                        }

                        foreach (DateTime reportDate in reportDates)
                        {
                            reportHeaders.Add(new ReportEntityHeaderModel
                            {
                                HeaderValue = reportDate.ToString("dd/MM/yyyy")
                            });
                        }

                        switch (model.Relationship)
                        {
                            case ReportRelationshipType.Employee:
                            reportEntitiesList = jobsList.GroupBy(x => x.SDKSiteId)
                                .Select(x => new ReportEntityModel()
                                {
                                    EntityName = sitesList.FirstOrDefault(y => y.SiteId == x.Key)?.SiteName,
                                    EntityId = x.Key,
                                    TimePerTimeUnit = reportDates.Select(z =>
                                            x
                                                .Where(j => j.DoneAt.Day == z.Day
                                                            && j.DoneAt.Month == z.Month
                                                            && j.DoneAt.Year == z.Year)
                                                .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                        )
                                        .ToList(),
                                    TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                })
                                .ToList();
                                break;
                            case ReportRelationshipType.Area:
                                reportEntitiesList = jobsList.GroupBy(x => new { x.AreaId, x.Area })
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = x.Key.Area?.Name,
                                        EntityId = x.Key.AreaId,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt.Day == z.Day
                                                                && j.DoneAt.Month == z.Month
                                                                && j.DoneAt.Year == z.Year)
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    })
                                    .ToList();
                                break;
                            case ReportRelationshipType.Machine:
                                reportEntitiesList = jobsList.GroupBy(x => new { x.MachineId, x.Machine })
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = x.Key.Machine?.Name,
                                        EntityId = x.Key.MachineId,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt.Day == z.Day
                                                                && j.DoneAt.Month == z.Month
                                                                && j.DoneAt.Year == z.Year)
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    })
                                    .ToList();
                                break;
                            case ReportRelationshipType.EmployeeMachine:
                                reportEntitiesList = jobsList.GroupBy(x => new { x.SDKSiteId, x.MachineId, x.Machine })
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = sitesList.FirstOrDefault(y => y.SiteId == x.Key.SDKSiteId)?.SiteName,
                                        EntityId = x.Key.SDKSiteId,
                                        RelatedEntityId = x.Key.MachineId,
                                        RelatedEntityName = x.Key.Machine.Name,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt.Day == z.Day
                                                                && j.DoneAt.Month == z.Month
                                                                && j.DoneAt.Year == z.Year)
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    }).ToList();
                                break;
                            case ReportRelationshipType.EmployeeArea:
                                reportEntitiesList = jobsList.GroupBy(x => new { x.SDKSiteId, x.AreaId, x.Area })
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = sitesList.FirstOrDefault(y => y.SiteId == x.Key.SDKSiteId)?.SiteName,
                                        EntityId = x.Key.SDKSiteId,
                                        RelatedEntityId = x.Key.AreaId,
                                        RelatedEntityName = x.Key.Area.Name,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt.Day == z.Day
                                                                && j.DoneAt.Month == z.Month
                                                                && j.DoneAt.Year == z.Year)
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    }).ToList();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        
                        // Employee - Machine
                        break;
                    case ReportType.Week:

                        DateTime firstDayOfWeek = model.DateFrom.FirstDayOfWeek(DayOfWeek.Monday);
                        DateTime lastDayOfWeek = model.DateTo.LastDayOfWeek(DayOfWeek.Monday);

                        DateTime dateFromWeek = new DateTime(
                            firstDayOfWeek.Year,
                            firstDayOfWeek.Month,
                            firstDayOfWeek.Day,
                            0,0,0);

                        DateTime dateToWeek = new DateTime(
                            lastDayOfWeek.Year,
                            lastDayOfWeek.Month,
                            lastDayOfWeek.Day,
                            23,59,59);

                        for (DateTime date = dateFromWeek; date <= dateToWeek; date = date.AddDays(7))
                        {
                            reportDates.Add(date);
                        }

                        foreach (DateTime reportDate in reportDates)
                        {
                            reportHeaders.Add(new ReportEntityHeaderModel
                            {
                                HeaderValue = $"W{DatesHelper.GetIso8601WeekOfYear(reportDate)}-{reportDate:yy}"
                            });
                        }

                        switch (model.Relationship)
                        {
                            case ReportRelationshipType.Employee:
                                reportEntitiesList = jobsList.GroupBy(x => x.SDKSiteId)
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = sitesList.FirstOrDefault(y => y.SiteId == x.Key)?.SiteName,
                                        EntityId = x.Key,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt >= z
                                                                && j.DoneAt < z.AddDays(7))
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    })
                                    .ToList();
                                break;
                            case ReportRelationshipType.Area:
                                reportEntitiesList = jobsList.GroupBy(x => new { x.AreaId, x.Area })
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = x.Key.Area?.Name,
                                        EntityId = x.Key.AreaId,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt >= z
                                                                && j.DoneAt < z.AddDays(7))
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    })
                                    .ToList();
                                break;
                            case ReportRelationshipType.Machine:
                                reportEntitiesList = jobsList.GroupBy(x => new { x.MachineId, x.Machine })
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = x.Key.Machine?.Name,
                                        EntityId = x.Key.MachineId,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt >= z
                                                                && j.DoneAt < z.AddDays(7))
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    })
                                    .ToList();
                                break;
                            case ReportRelationshipType.EmployeeArea:
                                reportEntitiesList = jobsList.GroupBy(x => new { x.SDKSiteId, x.AreaId, x.Area })
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = sitesList.FirstOrDefault(y => y.SiteId == x.Key.SDKSiteId)?.SiteName,
                                        EntityId = x.Key.SDKSiteId,
                                        RelatedEntityId = x.Key.AreaId,
                                        RelatedEntityName = x.Key.Area.Name,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt >= z
                                                                && j.DoneAt < z.AddDays(7))
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    }).ToList();
                                break;
                            case ReportRelationshipType.EmployeeMachine:
                                reportEntitiesList = jobsList.GroupBy(x => new { x.SDKSiteId, x.MachineId, x.Machine })
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = sitesList.FirstOrDefault(y => y.SiteId == x.Key.SDKSiteId)?.SiteName,
                                        EntityId = x.Key.SDKSiteId,
                                        RelatedEntityId = x.Key.MachineId,
                                        RelatedEntityName = x.Key.Machine.Name,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt >= z
                                                                && j.DoneAt < z.AddDays(7))
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    }).ToList();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case ReportType.Month:

                        DateTime firstDayOfMonth = model.DateFrom.FirstDayOfMonth();
                        DateTime lastDayOfMonth = model.DateTo.LastDayOfMonth();

                        DateTime dateFromMonth = new DateTime(
                            firstDayOfMonth.Year,
                            firstDayOfMonth.Month,
                            firstDayOfMonth.Day,
                            0,0,0);

                        DateTime dateToMonth = new DateTime(
                            lastDayOfMonth.Year,
                            lastDayOfMonth.Month,
                            lastDayOfMonth.Day,
                            23,59,59);

                        // Fill dates array to arrange time by time unit
                        for (DateTime date = dateFromMonth; date <= dateToMonth; date = date.AddMonths(1))
                        {
                            reportDates.Add(date);
                        }

                        // Add headers with required format
                        foreach (DateTime reportDate in reportDates)
                        {
                            reportHeaders.Add(new ReportEntityHeaderModel
                            {
                                HeaderValue = reportDate.ToString("MM/yyyy")
                            });
                        }

                        switch (model.Relationship)
                        {
                            case ReportRelationshipType.Employee:
                                reportEntitiesList = jobsList.GroupBy(x => x.SDKSiteId)
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = sitesList.FirstOrDefault(y => y.SiteId == x.Key)?.SiteName,
                                        EntityId = x.Key,
                                        // Fill dates array foreach date and calc sum for this date
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt >= z
                                                                && j.DoneAt < z.AddMonths(1))
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    })
                                    .ToList();
                                break;
                            case ReportRelationshipType.Area:
                                reportEntitiesList = jobsList.GroupBy(x => new { x.AreaId, x.Area })
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = x.Key.Area?.Name,
                                        EntityId = x.Key.AreaId,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt >= z
                                                                && j.DoneAt < z.AddMonths(1))
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    })
                                    .ToList();
                                break;
                            case ReportRelationshipType.Machine:
                                reportEntitiesList = jobsList.GroupBy(x => new { x.MachineId, x.Machine })
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = x.Key.Machine?.Name,
                                        EntityId = x.Key.MachineId,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt >= z
                                                                && j.DoneAt < z.AddMonths(1))
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    })
                                    .ToList();
                                break;
                            case ReportRelationshipType.EmployeeArea:
                                reportEntitiesList = jobsList.GroupBy(x => new { x.SDKSiteId, x.AreaId, x.Area })
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = sitesList.FirstOrDefault(y => y.SiteId == x.Key.SDKSiteId)?.SiteName,
                                        EntityId = x.Key.SDKSiteId,
                                        RelatedEntityId = x.Key.AreaId,
                                        RelatedEntityName = x.Key.Area.Name,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt >= z
                                                                && j.DoneAt < z.AddMonths(1))
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    }).ToList();
                                break;
                            case ReportRelationshipType.EmployeeMachine:
                                reportEntitiesList = jobsList.GroupBy(x => new { x.SDKSiteId, x.MachineId, x.Machine })
                                    .Select(x => new ReportEntityModel()
                                    {
                                        EntityName = sitesList.FirstOrDefault(y => y.SiteId == x.Key.SDKSiteId)?.SiteName,
                                        EntityId = x.Key.SDKSiteId,
                                        RelatedEntityId = x.Key.MachineId,
                                        RelatedEntityName = x.Key.Machine.Name,
                                        TimePerTimeUnit = reportDates.Select(z =>
                                                x
                                                    .Where(j => j.DoneAt >= z
                                                                && j.DoneAt < z.AddMonths(1))
                                                    .Sum(s => (decimal)TimeSpan.FromSeconds(s.TimeInSeconds).TotalMinutes)
                                            )
                                            .ToList(),
                                        TotalTime = x.Sum(z => (decimal)TimeSpan.FromSeconds(z.TimeInSeconds).TotalMinutes)
                                    }).ToList();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                }

                if (model.Relationship == ReportRelationshipType.EmployeeArea 
                    || model.Relationship == ReportRelationshipType.EmployeeMachine)
                {
                    // Group reports by employee
                    List<IGrouping<int, ReportEntityModel>> groupedReports = reportEntitiesList.GroupBy(x => x.EntityId).ToList();
                    foreach (IGrouping<int, ReportEntityModel> groupedReport in groupedReports)
                    {
                        // Calculate sum for sub report
                        List<decimal> sumByTimeUnit = new List<decimal>();
                        foreach (ReportEntityModel reportEntity in groupedReport)
                        {
                            int i = 0;
                            foreach (decimal timePerTimeUnit in reportEntity.TimePerTimeUnit)
                            {
                                if (sumByTimeUnit.Count <= i)
                                {
                                    sumByTimeUnit.Add(timePerTimeUnit);
                                }
                                else
                                {
                                    sumByTimeUnit[i] += timePerTimeUnit;
                                }

                                i++;
                            }
                        }

                        // Push sub report to reports array
                        finalModel.SubReports.Add(new SubReportModel()
                        {
                            Entities = groupedReport.ToList(),
                            TotalTime = groupedReport.Sum(x => x.TotalTime),
                            TotalTimePerTimeUnit = sumByTimeUnit
                        });
                    }
                }
                else
                {
                    // Calculate only one sub report for employee/area/machine
                    List<decimal> sumByTimeUnit = new List<decimal>();
                    foreach (ReportEntityModel reportEntity in reportEntitiesList)
                    {
                        int i = 0;
                        foreach (decimal timePerTimeUnit in reportEntity.TimePerTimeUnit)
                        {
                            if (sumByTimeUnit.Count <= i)
                            {
                                sumByTimeUnit.Add(timePerTimeUnit);
                            }
                            else
                            {
                                sumByTimeUnit[i] += timePerTimeUnit;
                            }

                            i++;
                        }
                    }

                    finalModel.SubReports = new List<SubReportModel>()
                    {
                        new SubReportModel()
                        {
                            Entities = reportEntitiesList,
                            TotalTime = reportEntitiesList.Sum(x => x.TotalTime),
                            TotalTimePerTimeUnit = sumByTimeUnit
                        }
                    };
                }

                finalModel.ReportHeaders = reportHeaders;
                finalModel.Relationship = model.Relationship;

                return new OperationDataResult<ReportModel>(true, finalModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<ReportModel>(false,
                    _machineAreaLocalizationService.GetString("ErrorWhileGeneratingReport"));
            }
        }

        public async Task<OperationDataResult<FileStreamModel>> GenerateReportFile(GenerateReportModel model)
        {
            string excelFile = null;
            try
            {
                OperationDataResult<ReportModel> reportDataResult = await GenerateReport(model);
                if (!reportDataResult.Success)
                {
                    return new OperationDataResult<FileStreamModel>(false, reportDataResult.Message);
                }

                excelFile = _excelService.CopyTemplateForNewAccount("report_template");
                bool writeResult = _excelService.WriteRecordsExportModelsToExcelFile(
                    reportDataResult.Model,
                    model,
                    excelFile);

                if (!writeResult)
                {
                    throw new Exception($"Error while writing excel file {excelFile}");
                }

                FileStreamModel result = new FileStreamModel()
                {
                    FilePath = excelFile,
                    FileStream = new FileStream(excelFile, FileMode.Open),
                };

                return new OperationDataResult<FileStreamModel>(true, result);
            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(excelFile) && File.Exists(excelFile))
                {
                    File.Delete(excelFile);
                }

                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<FileStreamModel>(
                    false,
                    _machineAreaLocalizationService.GetString("ErrorWhileGeneratingReportFile"));
            }
        }
    }
}