using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Enums;
using MachineArea.Pn.Infrastructure.Extensions;
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
                var core = _coreHelper.GetCore();
                var sitesList = core.SiteReadAll(false);
                var modelDateFrom = new DateTime(
                    model.DateFrom.Year,
                    model.DateFrom.Month,
                    model.DateFrom.Day,
                    0,0,0);

                var modelDateTo = new DateTime(
                    model.DateTo.Year,
                    model.DateTo.Month,
                    model.DateTo.Day,
                    23,59,59);

                var jobsList = await _dbContext.MachineAreaTimeRegistrations
                    .Include(x => x.Machine)
                    .Include(x => x.Area)
                    .Where(x => x.DoneAt >= modelDateFrom && x.DoneAt <= modelDateTo)
                    .ToListAsync();
                var reportEntitiesList = new List<ReportEntityModel>();
                var reportDates = new List<DateTime>();
                var reportHeaders = new List<ReportEntityHeaderModel>();
                switch (model.Type)
                {
                    case ReportType.Day:
                        var dateFrom = new DateTime(
                            model.DateFrom.Year,
                            model.DateFrom.Month,
                            model.DateFrom.Day,
                            0,0,0);

                        var dateTo = new DateTime(
                            model.DateTo.Year,
                            model.DateTo.Month,
                            model.DateTo.Day,
                            23,59,59);

                        for (var date = dateFrom; date <= dateTo; date = date.AddDays(1))
                        {
                            reportDates.Add(date);
                        }

                        foreach (var reportDate in reportDates)
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

                        var firstDayOfWeek = model.DateFrom.FirstDayOfWeek(DayOfWeek.Monday);
                        var lastDayOfWeek = model.DateTo.LastDayOfWeek(DayOfWeek.Monday);

                        var dateFromWeek = new DateTime(
                            firstDayOfWeek.Year,
                            firstDayOfWeek.Month,
                            firstDayOfWeek.Day,
                            0,0,0);

                        var dateToWeek = new DateTime(
                            lastDayOfWeek.Year,
                            lastDayOfWeek.Month,
                            lastDayOfWeek.Day,
                            23,59,59);

                        for (var date = dateFromWeek; date <= dateToWeek; date = date.AddDays(7))
                        {
                            reportDates.Add(date);
                        }

                        foreach (var reportDate in reportDates)
                        {
                            reportHeaders.Add(new ReportEntityHeaderModel
                            {
                                HeaderValue = $"{reportDate:dd/MM/yyyy} - {reportDate.AddDays(7):dd/MM/yyyy}"
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

                        var firstDayOfMonth = model.DateFrom.FirstDayOfMonth();
                        var lastDayOfMonth = model.DateTo.LastDayOfMonth();

                        var dateFromMonth = new DateTime(
                            firstDayOfMonth.Year,
                            firstDayOfMonth.Month,
                            firstDayOfMonth.Day,
                            0,0,0);

                        var dateToMonth = new DateTime(
                            lastDayOfMonth.Year,
                            lastDayOfMonth.Month,
                            lastDayOfMonth.Day,
                            23,59,59);

                        for (var date = dateFromMonth; date <= dateToMonth; date = date.AddMonths(1))
                        {
                            reportDates.Add(date);
                        }

                        foreach (var reportDate in reportDates)
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

                var sumByTimeUnit = new List<decimal>();
                foreach (var reportEntity in reportEntitiesList)
                {
                    var i = 0;
                    foreach (var timePerTimeUnit in reportEntity.TimePerTimeUnit)
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

                var finalModel = new ReportModel()
                {
                    Entities = reportEntitiesList,
                    ReportHeaders = reportHeaders,
                    TotalTime = reportEntitiesList
                        .Sum(x => x.TotalTime),
                    TotalTimePerTimeUnit = sumByTimeUnit
                };

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
                var reportDataResult = await GenerateReport(model);
                if (!reportDataResult.Success)
                {
                    return new OperationDataResult<FileStreamModel>(false, reportDataResult.Message);
                }

                excelFile = _excelService.CopyTemplateForNewAccount("report_template.xlsx");
                var writeResult = _excelService.WriteRecordsExportModelsToExcelFile(
                    reportDataResult.Model,
                    model,
                    excelFile);

                if (!writeResult)
                {
                    throw new Exception($"Error while writing excel file {excelFile}");
                }

                var result = new FileStreamModel()
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