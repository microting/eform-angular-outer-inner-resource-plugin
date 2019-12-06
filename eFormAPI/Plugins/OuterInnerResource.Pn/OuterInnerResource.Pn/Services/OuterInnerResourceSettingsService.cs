using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eForm.Infrastructure.Constants;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Models.OuterResources;
using OuterInnerResource.Pn.Infrastructure.Models.Settings;
using OuterInnerResource.Pn.Messages;
using Rebus.Bus;

namespace OuterInnerResource.Pn.Services
{
    public class OuterInnerResourceSettingsService : IOuterInnerResourceSettingsService
    {
        private readonly ILogger<OuterInnerResourceSettingsService> _logger;
        private readonly IOuterInnerResourceLocalizationService _outerInnerResourceLocalizationService;
        private readonly OuterInnerResourcePnDbContext _dbContext;
        private readonly IPluginDbOptions<OuterInnerResourceSettings> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBus _bus;

        public OuterInnerResourceSettingsService(
            ILogger<OuterInnerResourceSettingsService> logger,
            OuterInnerResourcePnDbContext dbContext,
            IOuterInnerResourceLocalizationService outerInnerResourceLocalizationService,
            IPluginDbOptions<OuterInnerResourceSettings> options,
            IHttpContextAccessor httpContextAccessor, 
            IRebusService rebusService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _outerInnerResourceLocalizationService = outerInnerResourceLocalizationService;
            _options = options;
            _httpContextAccessor = httpContextAccessor;
            _bus = rebusService.GetBus();
        }

        public async Task<OperationDataResult<OuterInnerResourceSettings>> GetSettings()
        {
            try
            {
                OuterInnerResourceSettings option = _options.Value;
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

                    string dbNameSection = Regex.Match(connectionString, @"(Database=(...)_eform-angular-outer-inner-resource-plugin;)").Groups[0].Value;
                    string dbPrefix = Regex.Match(connectionString, @"Database=(\d*)_").Groups[1].Value;
                    string sdk = $"Database={dbPrefix}_SDK;";
                    connectionString = connectionString.Replace(dbNameSection, sdk);
                    await _options.UpdateDb(settings => { settings.SdkConnectionString = connectionString;}, _dbContext, UserId);
                }

                return new OperationDataResult<OuterInnerResourceSettings>(true, option);
            }
            catch(Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<OuterInnerResourceSettings>(false,
                    _outerInnerResourceLocalizationService.GetString("ErrorWhileObtainingTrashInspectionSettings") + e.Message);
            }
        }

        public async Task<OperationResult> UpdateSettings(OuterInnerResourceSettings machineAreaSettingsModel)
        {
            try
            {
                string lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.EnabledSiteIds.ToString()}"; 
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
                    _outerInnerResourceLocalizationService.GetString("SettingsHaveBeenUpdatedSuccessfully"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _outerInnerResourceLocalizationService.GetString("ErrorWhileUpdatingSettings"));
            }
        }

        public OperationDataResult<List<int>> GetSitesEnabled()
        {
            string lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.EnabledSiteIds.ToString()}"; 
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
            string lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.EnabledSiteIds.ToString()}"; 
            string oldSdkSiteIds = _dbContext.PluginConfigurationValues
                .FirstOrDefault(x => 
                    x.Name == lookup)?.Value;

            string sdkSiteIds = "";
            int i = 0;
            
            foreach (int siteId in siteIds)
            {
                if (i > 0)
                    sdkSiteIds += ",";
                sdkSiteIds += siteId.ToString();
                i++;
            }

            await _options.UpdateDb(settings => { settings.EnabledSiteIds = sdkSiteIds; }, _dbContext, UserId);
            
            return new OperationResult(true);
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
            List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> 
                outerInnerResources = _dbContext.OuterInnerResources.Where(x => 
                        x.WorkflowState != Constants.WorkflowStates.Removed)
                .ToList();
            foreach (var outerInnerResource in outerInnerResources)
            {
                _bus.SendLocal(new OuterInnerResourceUpdate(outerInnerResource.Id));
            }
        }
    }
}
