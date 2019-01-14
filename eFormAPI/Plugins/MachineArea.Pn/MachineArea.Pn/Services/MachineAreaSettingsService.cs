using System;
using System.Diagnostics;
using System.Linq;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Data;
using MachineArea.Pn.Infrastructure.Data.Entities;
using MachineArea.Pn.Infrastructure.Models;
using eFormCore;
using eFormData;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Services
{
    public class MachineAreaSettingsService : IMachineAreaSettingsService
    {
        private readonly ILogger<MachineAreaSettingsService> _logger;
        private readonly IMachineAreaLocalizationService _machineAreaLocalizationService;
        private readonly MachineAreaPnDbContext _dbContext;
        private readonly IEFormCoreService _coreHelper;
        
        public MachineAreaSettingsService(ILogger<MachineAreaSettingsService> logger,
            MachineAreaPnDbContext dbContext,
            IEFormCoreService coreHelper,
            IMachineAreaLocalizationService machineAreaLocalizationService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _coreHelper = coreHelper;
            _machineAreaLocalizationService = machineAreaLocalizationService;
        }

        public OperationDataResult<MachineAreaSettingsModel> GetSettings()
        {
            try
            {
                MachineAreaSettingsModel result = new MachineAreaSettingsModel();
                MachineAreaSetting machineAreaSettings = _dbContext.MachineAreaSettings.FirstOrDefault();
                if(machineAreaSettings.SelectedeFormId != null)
                {
                    result.SelectedeFormId = (int)machineAreaSettings.SelectedeFormId;
                    result.SelectedeFormName = machineAreaSettings.SelectedeFormName;
                }
                else
                {
                    result.SelectedeFormId = null;
                }

                return new OperationDataResult<MachineAreaSettingsModel>(true, result);
            }
            catch(Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<MachineAreaSettingsModel>(false,
                    _machineAreaLocalizationService.GetString("ErrorWhileOptainingMachineareaSettings"));
            }
        }
        
        public OperationResult UpdateSettings(MachineAreaSettingsModel machineAreaSettingsModel)
        {
            try
            {
                if (machineAreaSettingsModel.SelectedeFormId == 0)
                {
                    return new OperationResult(true);
                }
                MachineAreaSetting machineAreaSettings = _dbContext.MachineAreaSettings.FirstOrDefault();
                if (machineAreaSettings == null)
                {
                    machineAreaSettings = new MachineAreaSetting()
                    {
                        SelectedeFormId = machineAreaSettingsModel.SelectedeFormId,
                    };
                    _dbContext.MachineAreaSettings.Add(machineAreaSettings);
                }
                else
                {
                    machineAreaSettings.SelectedeFormId = machineAreaSettingsModel.SelectedeFormId;
                }

                if (machineAreaSettingsModel.SelectedeFormId != null) 
                {
                    Core core = _coreHelper.GetCore();
                    MainElement template = core.TemplateRead((int)machineAreaSettingsModel.SelectedeFormId);
                    machineAreaSettings.SelectedeFormName = template.Label;
                }

                _dbContext.SaveChanges();
                return new OperationResult(true,
                    _machineAreaLocalizationService.GetString("SettingsHasBeenUpdatedSuccesfully"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _machineAreaLocalizationService.GetString("ErrorWhileUpdatingMachineAreaSettings"));
            }
        }
    }
}
