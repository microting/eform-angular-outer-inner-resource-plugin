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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eFormCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eForm.Dto;
using Microting.eForm.Infrastructure.Constants;
using Microting.eForm.Infrastructure.Data.Entities;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Enums;
using OuterInnerResource.Pn.Infrastructure.Helpers;
using OuterInnerResource.Pn.Infrastructure.Models;
using OuterInnerResource.Pn.Infrastructure.Models.Report;
using OuterInnerResource.Pn.Infrastructure.Models.Settings;
using Sentry;

namespace OuterInnerResource.Pn.Services;

public class OuterInnerResourceReportService(
    ILogger<OuterInnerResourceReportService> logger,
    OuterInnerResourcePnDbContext dbContext,
    IEFormCoreService coreHelper,
    IOuterInnerResourceLocalizationService outerInnerResourceLocalizationService,
    IPluginDbOptions<OuterInnerResourceSettings> options,
    IExcelService excelService)
    : IOuterInnerResourceReportService
{
    public OperationDataResult<ReportNamesModel> GetReportNames()
    {
        var reportNamesModel = new ReportNamesModel
        {
            ReportNameModels = new List<ReportNameModel>()
        };
        var outerResourceName = options.Value.OuterResourceName;
        var innerResourceName = options.Value.InnerResourceName;

        reportNamesModel.ReportNameModels.Add(new ReportNameModel
        {
            Id = 1,
            Name = outerInnerResourceLocalizationService.GetString("Employee")
        });
        reportNamesModel.ReportNameModels.Add(new ReportNameModel
        {
            Id = 2,
            Name = innerResourceName
        });
        reportNamesModel.ReportNameModels.Add(new ReportNameModel
        {
            Id = 3,
            Name = outerResourceName
        });
        reportNamesModel.ReportNameModels.Add(new ReportNameModel
        {
            Id = 4,
            Name = outerInnerResourceLocalizationService.GetString("Employee") + "-" + outerResourceName
        });
        reportNamesModel.ReportNameModels.Add(new ReportNameModel
        {
            Id = 5,
            Name = outerInnerResourceLocalizationService.GetString("Employee") + "-" + innerResourceName
        });
        reportNamesModel.ReportNameModels.Add(new ReportNameModel
        {
            Id = 6,
            Name = outerInnerResourceLocalizationService.GetString("Employee") + "-Total"
        });
        reportNamesModel.ReportNameModels.Add(new ReportNameModel
        {
            Id = 7,
            Name = $"{outerResourceName} {innerResourceName}",
        });
        reportNamesModel.ReportNameModels.Add(new ReportNameModel
        {
            Id = 8,
            Name = $"{innerResourceName} {outerResourceName}",
        });

        return new OperationDataResult<ReportNamesModel>(true, reportNamesModel);
    }

    public async Task<OperationDataResult<ReportModel>> GenerateReport(GenerateReportModel model)
    {
        try
        {
            var reportTimeType = options.Value.ReportTimeType;
            Core core = await coreHelper.GetCore();
            var sdkDbContext = core.DbContextHelper.GetDbContext();
            List<Site> sites = await sdkDbContext.Sites
                .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed).ToListAsync();
            // List<SiteDto> sitesList = await core.SiteReadAll(false);

            DateTime modelDateFrom = new DateTime(
                model.DateFrom.Year,
                model.DateFrom.Month,
                model.DateFrom.Day,
                0, 0, 0);
            DateTime modelDateTo = new DateTime(
                model.DateTo.Year,
                model.DateTo.Month,
                model.DateTo.Day,
                23, 59, 59);

            // results to exclude

            string outerResourceName = "";
            OuterResource areaToExclude;
            InnerResource machineToExclude;
            string innerResourceName = "";
            List<ResourceTimeRegistration> jobsList;

            outerResourceName = dbContext.PluginConfigurationValues
                .FirstOrDefault(x => x.Name == "OuterInnerResourceSettings:OuterTotalTimeName")?.Value;
            areaToExclude = await dbContext.OuterResources.FirstOrDefaultAsync(x => x.Name == outerResourceName);
            innerResourceName = dbContext.PluginConfigurationValues
                .FirstOrDefault(x => x.Name == "OuterInnerResourceSettings:InnerTotalTimeName")?.Value;
            machineToExclude =
                await dbContext.InnerResources.FirstOrDefaultAsync(x => x.Name == innerResourceName);

            IQueryable<ResourceTimeRegistration> query = dbContext.ResourceTimeRegistrations
                .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                .Include(x => x.InnerResource)
                .Include(x => x.OuterResource);

            if (areaToExclude != null && machineToExclude != null)
            {
                if (model.Relationship == ReportRelationshipType.EmployeeTotal)
                {
                    query = query.Where(x =>
                        x.OuterResourceId == areaToExclude.Id && x.InnerResourceId == machineToExclude.Id);
                }
                else
                {
                    query = query.Where(x =>
                        x.OuterResourceId != areaToExclude.Id && x.InnerResourceId != machineToExclude.Id);
                }

                query = query
                    .Where(x => x.DoneAt >= modelDateFrom && x.DoneAt <= modelDateTo);
            }
            else
            {
                query = query
                    .Where(x => x.DoneAt >= modelDateFrom && x.DoneAt <= modelDateTo);
            }

            jobsList = await query.ToListAsync();

            ReportModel reportModel = ReportsHelper.GetReportData(model, jobsList, sites, reportTimeType);

            return new OperationDataResult<ReportModel>(true, reportModel);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationDataResult<ReportModel>(false,
                outerInnerResourceLocalizationService.GetString("ErrorWhileGeneratingReport"));
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

            string outerResourceName = "";
            string innerResourceName = "";
            try
            {
                outerResourceName = dbContext.PluginConfigurationValues
                    .FirstOrDefault(x => x.Name == "OuterInnerResourceSettings:OuterResourceName")?.Value;
                innerResourceName = dbContext.PluginConfigurationValues
                    .FirstOrDefault(x => x.Name == "OuterInnerResourceSettings:InnerResourceName")?.Value;
            }
            catch
            {
            }

            switch (reportDataResult.Model.Relationship)
            {
                case ReportRelationshipType.Employee:
                    reportDataResult.Model.HumanReadableName =
                        outerInnerResourceLocalizationService.GetString("Employee");
                    break;
                case ReportRelationshipType.InnerResource:
                    reportDataResult.Model.HumanReadableName = innerResourceName;
                    break;
                case ReportRelationshipType.OuterResource:
                    reportDataResult.Model.HumanReadableName = outerResourceName;
                    break;
                case ReportRelationshipType.EmployeeInnerResource:
                    reportDataResult.Model.HumanReadableName =
                        outerInnerResourceLocalizationService.GetString("Employee") + "-" + innerResourceName;
                    break;
                case ReportRelationshipType.EmployeeOuterResource:
                    reportDataResult.Model.HumanReadableName =
                        outerInnerResourceLocalizationService.GetString("Employee") + "-" + outerResourceName;
                    break;
                case ReportRelationshipType.EmployeeTotal:
                    reportDataResult.Model.HumanReadableName =
                        outerInnerResourceLocalizationService.GetString("Employee" + "-Total");
                    break;
                case ReportRelationshipType.OuterInnerResource:
                    reportDataResult.Model.HumanReadableName =
                        $"{outerResourceName} {innerResourceName}";
                    break;
                case ReportRelationshipType.InnerOuterResource:
                    reportDataResult.Model.HumanReadableName =
                        $"{innerResourceName} {outerResourceName}";
                    break;
            }

            excelFile = excelService.CopyTemplateForNewAccount("report_template");
            bool writeResult = excelService.WriteRecordsExportModelsToExcelFile(
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

            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationDataResult<FileStreamModel>(
                false,
                outerInnerResourceLocalizationService.GetString("ErrorWhileGeneratingReportFile"));
        }
    }
}