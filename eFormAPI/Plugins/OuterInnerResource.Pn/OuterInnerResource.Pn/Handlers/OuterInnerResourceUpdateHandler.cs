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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eFormCore;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Dto;
using Microting.eForm.Infrastructure.Constants;
using Microting.eForm.Infrastructure.Models;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using OuterInnerResource.Pn.Infrastructure.Helpers;
using OuterInnerResource.Pn.Infrastructure.Models.InnerResources;
using OuterInnerResource.Pn.Infrastructure.Models.OuterResources;
using OuterInnerResource.Pn.Messages;
using Rebus.Bus;
using Rebus.Handlers;

namespace OuterInnerResource.Pn.Handlers
{
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
            string lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.SdkeFormId.ToString()}"; 
            
            int eFormId = int.Parse(_dbContext.PluginConfigurationValues.AsNoTracking()
                .FirstOrDefault(x => 
                    x.Name == lookup)?.Value);

//            MainElement mainElement = await _core.TemplateRead(eFormId);
            List<Site_Dto> sites = new List<Site_Dto>();
            
            lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.EnabledSiteIds.ToString()}"; 
            string sdkSiteIds = _dbContext.PluginConfigurationValues.AsNoTracking()
                .FirstOrDefault(x => 
                    x.Name == lookup)?.Value;
            foreach (string siteId in sdkSiteIds.Split(","))
            {
                sites.Add(await _core.SiteRead(int.Parse(siteId)));
            }
            
            if (message.InnerResourceModel != null)
            {
                await UpdateFromInnerResource(message.InnerResourceModel, sites, eFormId);
            }
            else
            {
                await UpdateFromOuterResource(message.OuterResourceModel, sites, eFormId);
            }
        }

        private async Task UpdateFromInnerResource(InnerResourceModel innerResourceModel, List<Site_Dto> sites, int eFormId)
        {
            List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> outerInnerResources = _dbContext.OuterInnerResources.Where(x =>
                x.InnerResourceId == innerResourceModel.Id && x.WorkflowState != Constants.WorkflowStates.Removed).ToList();

            List<int> requestedOuterResourceIds = innerResourceModel.RelatedOuterResourcesIds;
            List<int> deployedOuterResourceIds = new List<int>();
            List<int> toBeDeployed = new List<int>();
            
            foreach (Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource outerInnerResource in outerInnerResources)
            {                
                deployedOuterResourceIds.Add(outerInnerResource.OuterResourceId);

                if (!innerResourceModel.RelatedOuterResourcesIds.Contains(outerInnerResource.OuterResourceId))
                {
                    foreach (OuterInnerResourceSite outerInnerResourceSite in outerInnerResource.OuterInnerResourceSites)
                    {
                        if (outerInnerResourceSite.MicrotingSdkCaseId != null)
                        {
                            await outerInnerResourceSite.Delete(_dbContext);
                            await _bus.SendLocal(new OuterInnerResourceDeleteFromServer(outerInnerResourceSite.Id));
//                            await DeleteRelationship(outerInnerResource.Id, (int)outerInnerResourceSite.MicrotingSdkCaseId);
                        }
                    }
                    
                    await outerInnerResource.Delete(_dbContext);
                }
            }

            if (requestedOuterResourceIds.Count != 0)
            {
                toBeDeployed.AddRange(requestedOuterResourceIds.Where(x => !deployedOuterResourceIds.Contains(x)));
            }

            foreach (int outerResourceId in toBeDeployed)
            {
                OuterResource outerResource = _dbContext.OuterResources.SingleOrDefault(x => x.Id == outerResourceId);
                if (outerResource != null)
                    await CreateRelationships(innerResourceModel.Id, outerResourceId, innerResourceModel.Name, outerResource.Name, sites,
                        eFormId);
            }
            
            // check for new site and add accordingly
            foreach (Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource outerInnerResource in
                outerInnerResources.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed))
            {
                await CreateRelationships(outerInnerResource.InnerResourceId, outerInnerResource.OuterResourceId, innerResourceModel.Name, outerInnerResource.OuterResource.Name, sites,
                    eFormId);
            }
        }

        private async Task UpdateFromOuterResource(OuterResourceModel outerResourceModel, List<Site_Dto> sites, int eFormId)
        {
            List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> outerInnerResources = _dbContext.OuterInnerResources.Where(x =>
                x.OuterResourceId == outerResourceModel.Id && x.WorkflowState != Constants.WorkflowStates.Removed).ToList();

            List<int> requestedInnerResourceIds = outerResourceModel.RelatedInnerResourcesIds;
            List<int> deployedInnerResourceIds = new List<int>();
            List<int> toBeDeployed = new List<int>();
            
            foreach (Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource outerInnerResource in outerInnerResources)
            {
                deployedInnerResourceIds.Add(outerInnerResource.InnerResourceId);
                
                if (!outerResourceModel.RelatedInnerResourcesIds.Contains(outerInnerResource.InnerResourceId))
                {
                    foreach (OuterInnerResourceSite outerInnerResourceSite in outerInnerResource.OuterInnerResourceSites)
                    {
                        if (outerInnerResourceSite.MicrotingSdkCaseId != null)
                        {
                            await outerInnerResourceSite.Delete(_dbContext);
                            await _bus.SendLocal(new OuterInnerResourceDeleteFromServer(outerInnerResourceSite.Id));
//                            await DeleteRelationship(outerInnerResource.Id, (int)outerInnerResourceSite.MicrotingSdkCaseId);
                        }
                    }
                    await outerInnerResource.Delete(_dbContext);
                }
            }

            if (requestedInnerResourceIds.Count != 0)
            {
                toBeDeployed.AddRange(requestedInnerResourceIds.Where(x => !deployedInnerResourceIds.Contains(x)));
            }

            foreach (int innerResourceId in toBeDeployed)
            {
                InnerResource innerResource = _dbContext.InnerResources.SingleOrDefault(x => x.Id == innerResourceId);
                if (innerResource != null)
                    await CreateRelationships(innerResourceId, outerResourceModel.Id, innerResource.Name, outerResourceModel.Name, sites,
                        eFormId);
            }

            // check for new site and add accordingly
            foreach (Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource outerInnerResource in
                outerInnerResources.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed))
            {
                await CreateRelationships(outerInnerResource.InnerResourceId, outerInnerResource.OuterResourceId, outerInnerResource.InnerResource.Name, outerResourceModel.Name, sites,
                    eFormId);
            }
        }

        private async Task CreateRelationships(int innerResourceId, int outerResourceId, string innerResourceName, string outerResourceName
            , List<Site_Dto> sites, int eFormId)
        {
            Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource outerInnerResource = 
                _dbContext.OuterInnerResources.SingleOrDefault(x =>
                    x.InnerResourceId == innerResourceId 
                    && x.OuterResourceId == outerResourceId);

            if (outerInnerResource != null)
            {
                if (outerInnerResource.WorkflowState != Constants.WorkflowStates.Created)
                {
                    outerInnerResource.WorkflowState = Constants.WorkflowStates.Created;
                    await outerInnerResource.Update(_dbContext);   
                }
            }
            else
            {
                outerInnerResource =
                    new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource()
                    {
                        OuterResourceId = outerResourceId,
                        InnerResourceId = innerResourceId
                    };
                await outerInnerResource.Create(_dbContext);
            }
            
//            mainElement.Label = innerResourceName;
//            mainElement.ElementList[0].Label = innerResourceName;
//            mainElement.EndDate = DateTime.Now.AddYears(10).ToUniversalTime();
//            mainElement.StartDate = DateTime.Now.ToUniversalTime();
//            mainElement.Repeated = 0;
            
//            string lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.QuickSyncEnabled.ToString()}"; 
//
//            bool quickSyncEnabled = _dbContext.PluginConfigurationValues.AsNoTracking()
//                                        .FirstOrDefault(x => 
//                                            x.Name == lookup)?.Value == "true";
//
//            if (quickSyncEnabled)
//            {
//                mainElement.EnableQuickSync = true;    
//            }
//            List<Folder_Dto> folderDtos = await _core.FolderGetAll(true);
//
//            bool folderAlreadyExist = false;
//            int _microtingUId = 0;
//            foreach (Folder_Dto folderDto in folderDtos)
//            {
//                if (folderDto.Name == outerResourceName)
//                {
//                    folderAlreadyExist = true;
//                    _microtingUId = (int)folderDto.MicrotingUId;
//                }
//            }
//
//            if (!folderAlreadyExist)
//            {
//                await _core.FolderCreate(outerResourceName, "", null);
//                folderDtos = await _core.FolderGetAll(true);
//            
//                foreach (Folder_Dto folderDto in folderDtos)
//                {
//                    if (folderDto.Name == outerResourceName)
//                    {
//                        _microtingUId = (int)folderDto.MicrotingUId;
//                    }
//                }
//            }
//            
//            mainElement.CheckListFolderName = _microtingUId.ToString();
            await UpdateSitesDeployed(outerInnerResource, sites, eFormId);

        }

        private async Task UpdateSitesDeployed(
            Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource outerInnerResource, List<Site_Dto> sites, int eFormId)
        {

            WriteLogEntry("OuterInnerResourceUpdateHandler: UpdateSitesDeployed called");
            List<int> siteIds = new List<int>();
            
            foreach (Site_Dto siteDto in sites)
            {
                siteIds.Add(siteDto.SiteId);
                OuterInnerResourceSite outerInnerResourceSite = _dbContext.OuterInnerResourceSites.SingleOrDefault(x =>
                    x.MicrotingSdkSiteId == siteDto.SiteId 
                    && x.OuterInnerResourceId == outerInnerResource.Id 
                    && x.WorkflowState == Constants.WorkflowStates.Created 
                    && x.MicrotingSdkCaseId == null);
                if (outerInnerResourceSite == null)
                {
//                    int? sdkCaseId = await _core.CaseCreate(mainElement, "", siteDto.SiteId);

//                    if (sdkCaseId != null)
//                    {
                        outerInnerResourceSite = new OuterInnerResourceSite
                        {
                            OuterInnerResourceId = outerInnerResource.Id,
                            MicrotingSdkSiteId = siteDto.SiteId,
//                            MicrotingSdkCaseId = (int) sdkCaseId,
                            MicrotingSdkeFormId = eFormId
                        };
                        await outerInnerResourceSite.Create(_dbContext);

//                    }    
                }
                await _bus.SendLocal(new OuterInnerResourcePosteForm(outerInnerResourceSite.Id, eFormId));

            }

            var sitesConfigured = _dbContext.OuterInnerResourceSites.Where(x => x.OuterInnerResourceId == outerInnerResource.Id).ToList();
            WriteLogEntry("OuterInnerResourceUpdateHandler: sitesConfigured looked up");

            foreach (OuterInnerResourceSite outerInnerResourceSite in sitesConfigured)
            {
                WriteLogEntry(
                    $"OuterInnerResourceUpdateHandler: Looking at outerInnerResourceSite {outerInnerResourceSite.Id} for microtingSiteId {outerInnerResourceSite.MicrotingSdkSiteId}");

                if (!siteIds.Contains(outerInnerResourceSite.MicrotingSdkSiteId))
                {
                    WriteLogEntry($"OuterInnerResourceUpdateHandler: {outerInnerResourceSite.MicrotingSdkSiteId} not found in the list, so calling delete.");

                    if (outerInnerResourceSite.MicrotingSdkCaseId != null)
                    {
                        await outerInnerResourceSite.Delete(_dbContext);
                        await _bus.SendLocal(new OuterInnerResourceDeleteFromServer(outerInnerResourceSite.Id));
//                        bool result = await _core.CaseDelete((int) outerInnerResourceSite.MicrotingSdkCaseId);
//                        if (result)
//                        {
//                            await outerInnerResourceSite.Delete(_dbContext);
//                        }    
                    }
                }
            }
        }

//        private async Task DeleteRelationship(int outerInnerResourceId, int microtingSdkCaseId)
//        {
//            OuterInnerResourceSite outerInnerResourceSite =  
//                _dbContext.OuterInnerResourceSites.SingleOrDefault(x => x.OuterInnerResourceId == outerInnerResourceId && x.MicrotingSdkCaseId == microtingSdkCaseId);
//            if (outerInnerResourceSite != null)
//            {
//                bool result = await _core.CaseDelete(microtingSdkCaseId);
//                if (result)
//                {
//                    await outerInnerResourceSite.Delete(_dbContext);
//                }
//            }
//        }

        private void WriteLogEntry(string message)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[DBG] " + message);
            Console.ForegroundColor = oldColor;
        }
    }
}