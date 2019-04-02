using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models;
using eFormCore;
using eFormData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

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
                List<MachineAreaSetting> machineAreaSettings = _dbContext.MachineAreaSettings.ToList();
                if (machineAreaSettings.Count < 8)
                {
                    MachineAreaSettingsModel.SettingCreateDefaults(_dbContext);                    
                    machineAreaSettings = _dbContext.MachineAreaSettings.AsNoTracking().ToList();
                }
                result.machineAreaSettingsList = new List<MachineAreaSettingModel>();
                foreach (MachineAreaSetting machineAreaSettingModel in machineAreaSettings)
                {
                    MachineAreaSettingModel settingModel = new MachineAreaSettingModel();
                    settingModel.Id = machineAreaSettingModel.Id;
                    settingModel.Name = machineAreaSettingModel.Name;
                    settingModel.Value = machineAreaSettingModel.Value;
                    result.machineAreaSettingsList.Add(settingModel);
                }

                return new OperationDataResult<MachineAreaSettingsModel>(true, result);
            }
            catch(Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<MachineAreaSettingsModel>(false,
                    _machineAreaLocalizationService.GetString("ErrorWhileObtainingTrashInspectionSettings"));
            }
        }

        public OperationResult UpdateSettings(MachineAreaSettingsModel machineAreaSettingsModel)
        {
            try
            {
                foreach (MachineAreaSettingModel settingsModel in machineAreaSettingsModel
                    .machineAreaSettingsList)
                {
                    settingsModel.Update(_dbContext);
                }
                
                return new OperationResult(true,
                    _machineAreaLocalizationService.GetString("SettingsHaveBeenUpdatedSuccessfully"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _machineAreaLocalizationService.GetString("ErrorWhileUpdatingSettings"));
            }
        }
    }
}
