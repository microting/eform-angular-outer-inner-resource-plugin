using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.Report;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormMachineAreaBase.Infrastructure.Data;

namespace MachineArea.Pn.Services
{
    public class MachineAreaReportService : IMachineAreaReportService
    {
        private readonly ILogger<MachineAreaReportService> _logger;
        private readonly IMachineAreaLocalizationService _machineAreaLocalizationService;
        private readonly MachineAreaPnDbContext _dbContext;
        private readonly IEFormCoreService _coreHelper;

        public MachineAreaReportService(ILogger<MachineAreaReportService> logger,
            MachineAreaPnDbContext dbContext,
            IEFormCoreService coreHelper,
            IMachineAreaLocalizationService machineAreaLocalizationService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _coreHelper = coreHelper;
            _machineAreaLocalizationService = machineAreaLocalizationService;
        }

        public async Task<OperationDataResult<ReportModel>> GenerateReport(GenerateReportModel model)
        {
            try
            {

                return new OperationDataResult<ReportModel>(true, new ReportModel());
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<ReportModel>(false,
                    _machineAreaLocalizationService.GetString("ErrorWhileOptainingMachineareaSettings"));
            }
        }

        public async Task<OperationResult> GenerateReportFile(GenerateReportModel model)
        {
            try
            {
                
                return new OperationResult(true,
                    _machineAreaLocalizationService.GetString(""));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _machineAreaLocalizationService.GetString(""));
            }
        }

    }
}
