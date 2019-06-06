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
using MachineArea.Pn.Infrastructure.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
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
        private readonly IPluginDbOptions<MachineAreaBaseSettings> _options;

        public MachineAreaReportService(ILogger<MachineAreaReportService> logger,
            MachineAreaPnDbContext dbContext,
            IEFormCoreService coreHelper,
            IMachineAreaLocalizationService machineAreaLocalizationService,
            IPluginDbOptions<MachineAreaBaseSettings> options,
            IExcelService excelService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _coreHelper = coreHelper;
            _machineAreaLocalizationService = machineAreaLocalizationService;
            _excelService = excelService;
            _options = options;
        }

        public async Task<OperationDataResult<ReportNamesModel>> GetReportNames()
        {
            ReportNamesModel reportNamesModel = new ReportNamesModel();
            reportNamesModel.ReportNameModels = new List<ReportNameModel>();
            string outerResourceName = "";
            string innerResourceName = "";
            try
            {
                outerResourceName = _dbContext.PluginConfigurationValues.SingleOrDefault(x => x.Name == "MachineAreaBaseSettings:OuterResourceName").Value;
                innerResourceName = _dbContext.PluginConfigurationValues.SingleOrDefault(x => x.Name == "MachineAreaBaseSettings:InnerResourceName").Value; 
            }
            catch
            {
            }
            
            reportNamesModel.ReportNameModels.Add(new ReportNameModel
            {
                Id = 1,
                Name = _machineAreaLocalizationService.GetString("Employee")
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
                Name = _machineAreaLocalizationService.GetString("Employee") + "-"+outerResourceName
            });
            reportNamesModel.ReportNameModels.Add(new ReportNameModel
            {
                Id = 5,
                Name = _machineAreaLocalizationService.GetString("Employee") + "-"+innerResourceName
            });
            
            return new OperationDataResult<ReportNamesModel>(true, reportNamesModel);
        }

        public async Task<OperationDataResult<ReportModel>> GenerateReport(GenerateReportModel model)
        {
            try
            {
                var reportTimeType = _options.Value.ReportTimeType;
                Core core = _coreHelper.GetCore();
                List<Site_Dto> sitesList = core.SiteReadAll(false);

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

                List<MachineAreaTimeRegistration> jobsList = await _dbContext.MachineAreaTimeRegistrations
                    .Include(x => x.Machine)
                    .Include(x => x.Area)
                    .Where(x => x.DoneAt >= modelDateFrom && x.DoneAt <= modelDateTo)
                    .ToListAsync();

                ReportModel reportModel = ReportsHelper.GetReportData(model, jobsList, sitesList, reportTimeType);

                return new OperationDataResult<ReportModel>(true, reportModel);
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
                
                string outerResourceName = "";
                string innerResourceName = "";
                try
                {
                    outerResourceName = _dbContext.PluginConfigurationValues.SingleOrDefault(x => x.Name == "MachineAreaBaseSettings:OuterResourceName").Value;
                    innerResourceName = _dbContext.PluginConfigurationValues.SingleOrDefault(x => x.Name == "MachineAreaBaseSettings:InnerResourceName").Value; 
                }
                catch
                {
                }

                switch (reportDataResult.Model.Relationship)
                {
                    case ReportRelationshipType.Employee:
                        reportDataResult.Model.HumanReadableName =
                            _machineAreaLocalizationService.GetString("Employee");
                        break;
                    case ReportRelationshipType.Machine:
                        reportDataResult.Model.HumanReadableName = innerResourceName;
                        break;
                    case ReportRelationshipType.Area:
                        reportDataResult.Model.HumanReadableName = outerResourceName;
                        break;
                    case ReportRelationshipType.EmployeeMachine:
                        reportDataResult.Model.HumanReadableName =
                            _machineAreaLocalizationService.GetString("Employee") + "-" + innerResourceName;
                        break;
                    case ReportRelationshipType.EmployeeArea:
                        reportDataResult.Model.HumanReadableName =
                            _machineAreaLocalizationService.GetString("Employee") + "-" + outerResourceName;
                        break;
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