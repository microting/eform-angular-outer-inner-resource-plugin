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


using Microting.eForm.Infrastructure.Data.Entities;

namespace OuterInnerResource.Pn.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using eFormCore;
    using Infrastructure.Helpers;
    using Messages;
    using Microsoft.EntityFrameworkCore;
    using Microting.eForm.Dto;
    using Microting.eForm.Infrastructure.Constants;
    using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
    using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;
    using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
    using Rebus.Bus;
    using Rebus.Handlers;

    public class OuterInnerResourceUpdateHandler : IHandleMessages<OuterInnerResourceUpdate>
    {
        private readonly Core _core;
        private readonly OuterInnerResourcePnDbContext _dbContext;
        private readonly IBus _bus;

        public OuterInnerResourceUpdateHandler(Core core, DbContextHelper dbContextHelper, IBus bus)
        {
            _core = core;
            _dbContext = dbContextHelper.GetDbContext();
            _bus = bus;
        }

        #pragma warning disable 1998
        public async Task Handle(OuterInnerResourceUpdate message)
        {
            var lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.SdkeFormId.ToString()}";

            var sdkDbContext = _core.DbContextHelper.GetDbContext();
            var result = _dbContext.PluginConfigurationValues.AsNoTracking()
                .FirstOrDefault(x =>
                    x.Name == lookup)?.Value;
            if (int.TryParse(result, out var eFormId))
            {

                var sites = new List<Site>();

                lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.EnabledSiteIds.ToString()}";
                result = _dbContext.PluginConfigurationValues.AsNoTracking()
                    .FirstOrDefault(x =>
                        x.Name == lookup)?.Value;
                if (result != null)
                {
                    var sdkSiteIds = result;
                    foreach (var siteId in sdkSiteIds.Split(","))
                    {
                        if (int.TryParse(siteId, out var siteIdResultParse))
                        {
                            var site = await sdkDbContext.Sites.FirstOrDefaultAsync(x => x.MicrotingUid == siteIdResultParse);
                            if (site != null)
                            {
                                sites.Add(site);
                            }
                        }
                    }

                    var outerInnerResource =
                        await _dbContext.OuterInnerResources.FirstOrDefaultAsync(x =>
                            x.Id == message.OuterInnerResourceId);

                    await UpdateSitesDeployed(message.OldInnerResourceName, message.NewInnerResourceName, outerInnerResource, sites, eFormId);
                }
            }
        }

        private async Task UpdateSitesDeployed(string oldName, string newName,
            OuterInnerResource outerInnerResource, List<Site> sites, int eFormId)
        {

            WriteLogEntry("OuterInnerResourceUpdateHandler: UpdateSitesDeployed called");
            var siteIds = new List<int>();

            if (oldName != newName || oldName == null && newName == null)
            {
                var outerInnerResourceSites = await _dbContext.OuterInnerResourceSites.Where(
                    x => x.OuterInnerResourceId == outerInnerResource.Id
                        && x.WorkflowState == Constants.WorkflowStates.Created).ToListAsync();
                foreach (var outerInnerResourceSite in outerInnerResourceSites)
                {
                    if (outerInnerResourceSite.MicrotingSdkCaseId != null)
                    {
                        await _core.CaseDelete((int) outerInnerResourceSite.MicrotingSdkCaseId);
                        await outerInnerResourceSite.Delete(_dbContext);
                    }
                }
            }

            if (outerInnerResource.WorkflowState == Constants.WorkflowStates.Created)
            {
                if (sites.Any())
                {
                    foreach (var site in sites)
                    {
                        siteIds.Add(site.Id);
                        var outerInnerResourceSites = await _dbContext.OuterInnerResourceSites.Where(
                            x =>
                                x.MicrotingSdkSiteId == site.Id
                                && x.OuterInnerResourceId == outerInnerResource.Id
                                && x.WorkflowState == Constants.WorkflowStates.Created).ToListAsync();
                        if (!outerInnerResourceSites.Any())
                        {
                            Console.WriteLine("OuterInnerResourceUpdateHandler: Create new OuterInnerResourceSite for siteId: " + site.MicrotingUid);
                            var outerInnerResourceSite = new OuterInnerResourceSite
                            {
                                OuterInnerResourceId = outerInnerResource.Id,
                                MicrotingSdkSiteId = site.Id,
                                MicrotingSdkeFormId = eFormId
                            };
                            await outerInnerResourceSite.Create(_dbContext);
                            await _bus.SendLocal(new OuterInnerResourcePosteForm(outerInnerResourceSite.Id,
                                eFormId));
                        }
                        else
                        {
                            if (outerInnerResourceSites.First().MicrotingSdkCaseId == null)
                            {
                                Console.WriteLine("OuterInnerResourceUpdateHandler: Create new OuterInnerResourceSite for siteId: " + site.MicrotingUid + " and MicrotingSdkCaseId is null");
                                await _bus.SendLocal(new OuterInnerResourcePosteForm(
                                    outerInnerResourceSites.First().Id,
                                    eFormId));
                            }
                        }
                    }
                }
            }
            var sitesConfigured = _dbContext.OuterInnerResourceSites.Where(x =>
                x.OuterInnerResourceId == outerInnerResource.Id
                && x.WorkflowState != Constants.WorkflowStates.Removed).ToList();
            WriteLogEntry("OuterInnerResourceUpdateHandler: sitesConfigured looked up");

            if (sitesConfigured.Any())
            {
                foreach (var outerInnerResourceSite in sitesConfigured)
                {
                    if (!siteIds.Contains(outerInnerResourceSite.MicrotingSdkSiteId)
                        || outerInnerResource.WorkflowState == Constants.WorkflowStates.Removed)
                    {
                        if (outerInnerResourceSite.MicrotingSdkCaseId != null)
                        {
                            await outerInnerResourceSite.Delete(_dbContext);
                            await _bus.SendLocal(new OuterInnerResourceDeleteFromServer(outerInnerResourceSite.Id));
                        }
                    }
                }
            }
        }

        private void WriteLogEntry(string message)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[DBG] " + message);
            Console.ForegroundColor = oldColor;
        }
    }
}