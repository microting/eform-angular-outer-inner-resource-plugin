using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormMachineAreaBase.Infrastructure.Data;

namespace MachineArea.Pn.Services
{
    public class MachineAreaSettingsService : IMachineAreaSettingsService
    {
        private readonly ILogger<MachineAreaSettingsService> _logger;
        private readonly IMachineAreaLocalizationService _machineAreaLocalizationService;
        private readonly MachineAreaPnDbContext _dbContext;
        private readonly IPluginDbOptions<MachineAreaBaseSettings> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MachineAreaSettingsService(
            ILogger<MachineAreaSettingsService> logger,
            MachineAreaPnDbContext dbContext,
            IMachineAreaLocalizationService machineAreaLocalizationService,
            IPluginDbOptions<MachineAreaBaseSettings> options,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dbContext = dbContext;
            _machineAreaLocalizationService = machineAreaLocalizationService;
            _options = options;
            _httpContextAccessor = httpContextAccessor;
        }

        public OperationDataResult<MachineAreaBaseSettings> GetSettings()
        {
            try
            {
                var option = _options.Value;

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
                }, _dbContext, UserId);
                
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
                var value = _httpContextAccessor?.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return value == null ? 0 : int.Parse(value);
            }
        }
    }
}
