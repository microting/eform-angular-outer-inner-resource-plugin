using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.OuterResources;
using MachineArea.Pn.Infrastructure.Models.Settings;
using MachineArea.Pn.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eForm.Infrastructure.Constants;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using Rebus.Bus;

namespace MachineArea.Pn.Services
{
    public class OuterInnerResourceSettingsService : IMachineAreaSettingsService
    {
        private readonly ILogger<OuterInnerResourceSettingsService> _logger;
        private readonly IMachineAreaLocalizationService _machineAreaLocalizationService;
        private readonly OuterInnerResourcePnDbContext _dbContext;
        private readonly IPluginDbOptions<MachineAreaBaseSettings> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBus _bus;

        public OuterInnerResourceSettingsService(
            ILogger<OuterInnerResourceSettingsService> logger,
            OuterInnerResourcePnDbContext dbContext,
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

        public async Task<OperationDataResult<MachineAreaBaseSettings>> GetSettings()
        {
            try
            {
                MachineAreaBaseSettings option = _options.Value;
                if (option.Token == "...")
                {
                    string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    Random random = new Random();
                    string result = new string(chars.Select(c => chars[random.Next(chars.Length)]).Take(32).ToArray());
                    await _options.UpdateDb(settings => { settings.Token = result;}, _dbContext, UserId);
                }

                if (option.SdkConnectionString == "...")
                {
                    string connectionString = _dbContext.Database.GetDbConnection().ConnectionString;

                    string dbNameSection = Regex.Match(connectionString, @"(Database=(...)_eform-angular-\w*-plugin;)").Groups[0].Value;
                    string dbPrefix = Regex.Match(connectionString, @"Database=(\d*)_").Groups[1].Value;
                    string sdk = $"Database={dbPrefix}_SDK;";
                    connectionString = connectionString.Replace(dbNameSection, sdk);
                    await _options.UpdateDb(settings => { settings.SdkConnectionString = connectionString;}, _dbContext, UserId);
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
                string lookup = $"MachineAreaBaseSettings:{OuterInnerResourceSettingsEnum.EnabledSiteIds.ToString()}"; 
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

        public async Task<OperationDataResult<List<int>>> GetSitesEnabled()
        {
            string lookup = $"MachineAreaBaseSettings:{OuterInnerResourceSettingsEnum.EnabledSiteIds.ToString()}"; 
            string oldSdkSiteIds = _dbContext.PluginConfigurationValues
                .FirstOrDefault(x => 
                    x.Name == lookup)?.Value;
            List<int> siteIds = new List<int>();
            if (!string.IsNullOrEmpty(oldSdkSiteIds))
            {
                foreach (string s in oldSdkSiteIds.Split(","))
                {
                    siteIds.Add(int.Parse(s));
                }
            }
            
            return new OperationDataResult<List<int>>(true, siteIds);
//            throw new oldSdkSiteIds.sp;
        }

        public async Task<OperationResult> UpdateSitesEnabled(List<int> siteIds)
        {
            throw new NotImplementedException();
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
            List<OuterResource> areas = _dbContext.OuterResources.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                .ToList();
            foreach (OuterResource area in areas)
            {
                OuterResourceModel outerResourceModel = new OuterResourceModel()
                {
                    Id = area.Id,
                    RelatedInnerResourcesIds = _dbContext.OuterInnerResources.
                        Where(x => x.OuterResourceId == area.Id && 
                                   x.WorkflowState != Constants.WorkflowStates.Removed).
                        Select(x => x.InnerResourceId).ToList(),
                    Name = area.Name
                };
                        
                _bus.SendLocal(new OuterInnerResourceUpdate(null, outerResourceModel));
            }
        }
    }
}
