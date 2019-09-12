using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.Areas;
using MachineArea.Pn.Infrastructure.Models.Settings;
using MachineArea.Pn.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eForm.Infrastructure.Constants;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Consts;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;
using Rebus.Bus;

namespace MachineArea.Pn.Services
{
    public class MachineAreaSettingsService : IMachineAreaSettingsService
    {
        private readonly ILogger<MachineAreaSettingsService> _logger;
        private readonly IMachineAreaLocalizationService _machineAreaLocalizationService;
        private readonly MachineAreaPnDbContext _dbContext;
        private readonly IPluginDbOptions<MachineAreaBaseSettings> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBus _bus;

        public MachineAreaSettingsService(
            ILogger<MachineAreaSettingsService> logger,
            MachineAreaPnDbContext dbContext,
            IMachineAreaLocalizationService machineAreaLocalizationService,
            IPluginDbOptions<MachineAreaBaseSettings> options,
            IHttpContextAccessor httpContextAccessor, 
            IRebusService rebusService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _machineAreaLocalizationService = machineAreaLocalizationService;
            _options = options;
            _httpContextAccessor = httpContextAccessor;
            _bus = rebusService.GetBus();
        }

        public OperationDataResult<MachineAreaBaseSettings> GetSettings()
        {
            try
            {
                MachineAreaBaseSettings option = _options.Value;

                if (option.SdkConnectionString == "...")
                {
                    string connectionString = _dbContext.Database.GetDbConnection().ConnectionString;

                    string dbNameSection = Regex.Match(connectionString, @"(Database=(...)_eform-angular-\w*-plugin;)").Groups[0].Value;
                    string dbPrefix = Regex.Match(connectionString, @"Database=(\d*)_").Groups[1].Value;
                    string sdk = $"Database={dbPrefix}_SDK;";
                    connectionString = connectionString.Replace(dbNameSection, sdk);
                    _options.UpdateDb(settings => { settings.SdkConnectionString = connectionString;}, _dbContext, UserId);

                }

                return new OperationDataResult<MachineAreaBaseSettings>(true, option);
            }
            catch(Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<MachineAreaBaseSettings>(false,
                    _machineAreaLocalizationService.GetString("ErrorWhileObtainingTrashInspectionSettings"));
            }
        }

        public async Task<OperationResult> UpdateSettings(MachineAreaBaseSettings machineAreaSettingsModel)
        {
            try
            {
                string lookup = $"MachineAreaBaseSettings:{MachineAreaSettingsEnum.EnabledSiteIds.ToString()}"; 
                string oldSdkSiteIds = _dbContext.PluginConfigurationValues
                    .FirstOrDefault(x => 
                        x.Name == lookup)?.Value;
            
                await _options.UpdateDb(settings =>
                {
                    settings.EnabledSiteIds = machineAreaSettingsModel.EnabledSiteIds;
                    settings.LogLevel = machineAreaSettingsModel.LogLevel;
                    settings.LogLimit = machineAreaSettingsModel.LogLimit;
                    settings.MaxParallelism = machineAreaSettingsModel.MaxParallelism;
                    settings.NumberOfWorkers = machineAreaSettingsModel.NumberOfWorkers;
                    settings.SdkConnectionString = machineAreaSettingsModel.SdkConnectionString;
                    settings.SdkeFormId = machineAreaSettingsModel.SdkeFormId;
                    settings.Token = machineAreaSettingsModel.Token;
                    settings.ReportTimeType = machineAreaSettingsModel.ReportTimeType;
                    settings.OuterResourceName = machineAreaSettingsModel.OuterResourceName;
                    settings.InnerResourceName = machineAreaSettingsModel.InnerResourceName;
                    settings.OuterTotalTimeName = machineAreaSettingsModel.OuterTotalTimeName;
                    settings.InnerTotalTimeName = machineAreaSettingsModel.InnerTotalTimeName;
                    settings.ShouldCheckAllCases = machineAreaSettingsModel.ShouldCheckAllCases;
                    settings.QuickSyncEnabled = machineAreaSettingsModel.QuickSyncEnabled;
                }, _dbContext, UserId);

                if (oldSdkSiteIds != machineAreaSettingsModel.EnabledSiteIds)
                {
                    CreateNewSiteRelations();
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
        public int UserId
        {
            get
            {
                string value = _httpContextAccessor?.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return value == null ? 0 : int.Parse(value);
            }
        }

        private void CreateNewSiteRelations()
        {
            List<Area> areas = _dbContext.Areas.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                .ToList();
            foreach (Area area in areas)
            {
                AreaModel areaModel = new AreaModel()
                {
                    Id = area.Id,
                    RelatedMachinesIds = _dbContext.MachineAreas.
                        Where(x => x.AreaId == area.Id && 
                                   x.WorkflowState != Constants.WorkflowStates.Removed).
                        Select(x => x.MachineId).ToList(),
                    Name = area.Name
                };
                        
                _bus.SendLocal(new MachineAreaUpdate(null, areaModel));
            }
        }
    }
}
