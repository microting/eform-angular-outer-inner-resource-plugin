/*
The MIT License (MIT)

Copyright (c) 2007 - 2021 Microting A/S

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

using Sentry;

namespace OuterInnerResource.Pn.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eForm.Infrastructure.Constants;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;
using Microting.eFormApi.BasePn.Abstractions;
using Abstractions;
using Infrastructure.Models.Settings;
using Messages;
using Rebus.Bus;

public class OuterInnerResourceSettingsService(
    ILogger<OuterInnerResourceSettingsService> logger,
    OuterInnerResourcePnDbContext dbContext,
    IOuterInnerResourceLocalizationService outerInnerResourceLocalizationService,
    IPluginDbOptions<OuterInnerResourceSettings> options,
    IRebusService rebusService,
    IUserService userService)
    : IOuterInnerResourceSettingsService
{
    private readonly IBus _bus = rebusService.GetBus();

    public async Task<OperationDataResult<OuterInnerResourceSettings>> GetSettings()
    {
        try
        {
            var option = options.Value;
            if (option.Token == "...")
            {
                const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var result = new string(chars.Select(c => chars[random.Next(chars.Length)]).Take(32).ToArray());
                await options.UpdateDb(settings => { settings.Token = result; }, dbContext, userService.UserId);
            }

            if (option.SdkConnectionString == "...")
            {
                var connectionString = dbContext.Database.GetDbConnection().ConnectionString;

                var dbNameSection = Regex.Match(connectionString,
                    @"(Database=(...)_eform-angular-outer-inner-resource-plugin;)").Groups[0].Value;
                var dbPrefix = Regex.Match(connectionString, @"Database=(\d*)_").Groups[1].Value;
                var sdk = $"Database={dbPrefix}_SDK;";
                connectionString = connectionString.Replace(dbNameSection, sdk);
                await options.UpdateDb(settings => { settings.SdkConnectionString = connectionString; },
                    dbContext, userService.UserId);
            }

            return new OperationDataResult<OuterInnerResourceSettings>(true, option);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationDataResult<OuterInnerResourceSettings>(false,
                outerInnerResourceLocalizationService.GetString("ErrorWhileObtainingTrashInspectionSettings") +
                e.Message);
        }
    }

    public async Task<OperationResult> UpdateSettings(OuterInnerResourceSettings machineAreaSettingsModel)
    {
        try
        {
            var lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.EnabledSiteIds}";
            var oldSdkSiteIds = dbContext.PluginConfigurationValues
                .FirstOrDefault(x =>
                    x.Name == lookup)?.Value;

            await options.UpdateDb(settings =>
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
            }, dbContext, userService.UserId);

            CreateNewSiteRelations();

            return new OperationResult(true,
                outerInnerResourceLocalizationService.GetString("SettingsHaveBeenUpdatedSuccessfully"));
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationResult(false,
                outerInnerResourceLocalizationService.GetString("ErrorWhileUpdatingSettings"));
        }
    }

    public OperationDataResult<List<int>> GetSitesEnabled()
    {
        var lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.EnabledSiteIds}";
        var oldSdkSiteIds = dbContext.PluginConfigurationValues
            .FirstOrDefault(x =>
                x.Name == lookup)?.Value;
        var siteIds = new List<int>();
        if (!string.IsNullOrEmpty(oldSdkSiteIds))
        {
            foreach (var s in oldSdkSiteIds.Split(","))
            {
                siteIds.Add(int.Parse(s));
            }
        }

        return new OperationDataResult<List<int>>(true, siteIds);
    }

    public async Task<OperationResult> UpdateSitesEnabled(List<int> siteIds)
    {
        var sdkSiteIds = "";
        var i = 0;

        foreach (var siteId in siteIds)
        {
            if (i > 0)
                sdkSiteIds += ",";
            sdkSiteIds += siteId.ToString();
            i++;
        }

        await options.UpdateDb(settings => { settings.EnabledSiteIds = sdkSiteIds; }, dbContext,
            userService.UserId);

        CreateNewSiteRelations();

        return new OperationResult(true);
    }

    private void CreateNewSiteRelations()
    {
        var
            outerInnerResources = dbContext.OuterInnerResources.Where(x =>
                    x.WorkflowState != Constants.WorkflowStates.Removed)
                .ToList();
        foreach (var outerInnerResource in outerInnerResources)
        {
            _bus.SendLocal(new OuterInnerResourceUpdate(outerInnerResource.Id, null, null, null, null, null));
        }
    }
}