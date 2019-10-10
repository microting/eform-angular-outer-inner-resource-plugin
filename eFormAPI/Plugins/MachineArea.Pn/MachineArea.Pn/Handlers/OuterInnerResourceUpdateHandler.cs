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
using MachineArea.Pn.Infrastructure.Models.InnerResources;
using MachineArea.Pn.Infrastructure.Models.OuterResources;
using MachineArea.Pn.Messages;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Dto;
using Microting.eForm.Infrastructure.Constants;
using Microting.eForm.Infrastructure.Models;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using Rebus.Handlers;

namespace MachineArea.Pn.Handlers
{
    public class OuterInnerResourceUpdateHandler : IHandleMessages<OuterInnerResourceUpdate>
    {  
        private readonly Core _core;
        private readonly OuterInnerResourcePnDbContext _dbContext;        
        
        public OuterInnerResourceUpdateHandler(Core core, OuterInnerResourcePnDbContext context)
        {
            _core = core;
            _dbContext = context;
        }
        
        #pragma warning disable 1998
        public async Task Handle(OuterInnerResourceUpdate message)
        {            
            string lookup = $"MachineAreaBaseSettings:{OuterInnerResourceSettingsEnum.SdkeFormId.ToString()}"; 
            
            int eFormId = int.Parse(_dbContext.PluginConfigurationValues
                .FirstOrDefault(x => 
                    x.Name == lookup)?.Value);

            MainElement mainElement = _core.TemplateRead(eFormId);
            List<Site_Dto> sites = new List<Site_Dto>();
            
            lookup = $"MachineAreaBaseSettings:{OuterInnerResourceSettingsEnum.EnabledSiteIds.ToString()}"; 
            string sdkSiteIds = _dbContext.PluginConfigurationValues
                .FirstOrDefault(x => 
                    x.Name == lookup)?.Value;
            foreach (string siteId in sdkSiteIds.Split(","))
            {
                sites.Add(_core.SiteRead(int.Parse(siteId)));
            }
            
            if (message.InnerResourceModel != null)
            {
                await UpdateFromMachine(message.InnerResourceModel, mainElement, sites, eFormId);
            }
            else
            {
                await UpdateFromArea(message.OuterResourceModel, mainElement, sites, eFormId);
            }
        }

        private async Task UpdateFromMachine(InnerResourceModel innerResourceModel, MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
            List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> machineAreas = _dbContext.OuterInnerResources.Where(x =>
                x.InnerResourceId == innerResourceModel.Id && x.WorkflowState != Constants.WorkflowStates.Removed).ToList();

            List<int> requestedAreaIds = innerResourceModel.RelatedOuterResourcesIds;
            List<int> deployedAreaIds = new List<int>();
            List<int> toBedeployed = new List<int>();
            
            foreach (Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource machineArea in machineAreas)
            {                
                deployedAreaIds.Add(machineArea.OuterResourceId);

                if (!innerResourceModel.RelatedOuterResourcesIds.Contains(machineArea.OuterResourceId))
                {
                    foreach (OuterInnerResourceSite machineAreaSite in machineArea.OuterInnerResourceSites)
                    {
                        await DeleteRelationship(machineArea.Id, machineAreaSite.MicrotingSdkCaseId);
                    }
                    
                    await machineArea.Delete(_dbContext);
                }
            }

            if (requestedAreaIds.Count != 0)
            {
                toBedeployed.AddRange(requestedAreaIds.Where(x => !deployedAreaIds.Contains(x)));
            }

            foreach (int areaId in toBedeployed)
            {
                OuterResource area = _dbContext.OuterResources.SingleOrDefault(x => x.Id == areaId);
                if (area != null)
                    await CreateRelationships(innerResourceModel.Id, areaId, innerResourceModel.Name, area.Name, mainElement, sites,
                        eFormId);
            }
            
            // check for new site and add accordingly
            foreach (Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource machineArea in
                machineAreas.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed))
            {
                await CreateRelationships(machineArea.InnerResourceId, machineArea.OuterResourceId, innerResourceModel.Name, machineArea.OuterResource.Name, mainElement, sites,
                    eFormId);
            }
        }

        private async Task UpdateFromArea(OuterResourceModel outerResourceModel, MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
            List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> machineAreas = _dbContext.OuterInnerResources.Where(x =>
                x.OuterResourceId == outerResourceModel.Id && x.WorkflowState != Constants.WorkflowStates.Removed).ToList();

            List<int> requestedMachineIds = outerResourceModel.RelatedInnerResourcesIds;
            List<int> deployedMachineIds = new List<int>();
            List<int> toBedeployed = new List<int>();
            
            foreach (Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource machineArea in machineAreas)
            {
                deployedMachineIds.Add(machineArea.InnerResourceId);
                
                if (!outerResourceModel.RelatedInnerResourcesIds.Contains(machineArea.InnerResourceId))
                {
                    foreach (OuterInnerResourceSite machineAreaSite in machineArea.OuterInnerResourceSites)
                    {
                        await DeleteRelationship(machineArea.Id, machineAreaSite.MicrotingSdkCaseId);
                    }
                    await machineArea.Delete(_dbContext);
                }
            }

            if (requestedMachineIds.Count != 0)
            {
                toBedeployed.AddRange(requestedMachineIds.Where(x => !deployedMachineIds.Contains(x)));
            }

            foreach (int machineId in toBedeployed)
            {
                InnerResource machine = _dbContext.InnerResources.SingleOrDefault(x => x.Id == machineId);
                if (machine != null)
                    await CreateRelationships(machineId, outerResourceModel.Id, machine.Name, outerResourceModel.Name, mainElement, sites,
                        eFormId);
            }

            // check for new site and add accordingly
            foreach (Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource machineArea in
                machineAreas.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed))
            {
                await CreateRelationships(machineArea.InnerResourceId, machineArea.OuterResourceId, machineArea.InnerResource.Name, outerResourceModel.Name, mainElement, sites,
                    eFormId);
            }
        }

        private async Task CreateRelationships(int machineId, int areaId, string machineName, string areaName,
            MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
            Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource machineArea = _dbContext.OuterInnerResources.SingleOrDefault(x =>
                    x.InnerResourceId == machineId && x.OuterResourceId == areaId);

            if (machineArea != null)
            {
                if (machineArea.WorkflowState != Constants.WorkflowStates.Created)
                {
                    machineArea.WorkflowState = Constants.WorkflowStates.Created;
                    await machineArea.Update(_dbContext);   
                }
            }
            else
            {
                machineArea =
                    new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource()
                    {
                        OuterResourceId = areaId,
                        InnerResourceId = machineId
                    };
                await machineArea.Create(_dbContext);
            }
            
            mainElement.Label = machineName;
            mainElement.ElementList[0].Label = machineName;
            mainElement.EndDate = DateTime.Now.AddYears(10).ToUniversalTime();
            mainElement.StartDate = DateTime.Now.ToUniversalTime();
            mainElement.Repeated = 0;
            
            string lookup = $"MachineAreaBaseSettings:{OuterInnerResourceSettingsEnum.QuickSyncEnabled.ToString()}"; 

            bool quickSyncEnabled = _dbContext.PluginConfigurationValues.AsNoTracking()
                                        .FirstOrDefault(x => 
                                            x.Name == lookup)?.Value == "true";

            if (quickSyncEnabled)
            {
                mainElement.EnableQuickSync = true;    
            }
            List<Folder_Dto> folderDtos = _core.FolderGetAll(true);

            bool folderAlreadyExist = false;
            int _microtingUId = 0;
            foreach (Folder_Dto folderDto in folderDtos)
            {
                if (folderDto.Name == areaName)
                {
                    folderAlreadyExist = true;
                    _microtingUId = (int)folderDto.MicrotingUId;
                }
            }

            if (!folderAlreadyExist)
            {
                _core.FolderCreate(areaName, "", null);
                folderDtos = _core.FolderGetAll(true);
            
                foreach (Folder_Dto folderDto in folderDtos)
                {
                    if (folderDto.Name == areaName)
                    {
                        _microtingUId = (int)folderDto.MicrotingUId;
                    }
                }
            }
            
            mainElement.CheckListFolderName = _microtingUId.ToString();
            await UpdateSitesDeployed(machineArea, mainElement, sites, eFormId);

        }

        private async Task UpdateSitesDeployed(
            Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource machineArea,
            MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {

            WriteLogEntry("MachineAreaUpdateHandler: UpdateSitesDeployed called");
            List<int> siteIds = new List<int>();
            
            foreach (Site_Dto siteDto in sites)
            {
                siteIds.Add(siteDto.SiteId);
                OuterInnerResourceSite siteMatch = _dbContext.OuterInnerResourceSites.SingleOrDefault(x =>
                    x.MicrotingSdkSiteId == siteDto.SiteId && x.OuterInnerResourceId == machineArea.Id && x.WorkflowState == Constants.WorkflowStates.Created);
                if (siteMatch == null)
                {
                    int? sdkCaseId = _core.CaseCreate(mainElement, "", siteDto.SiteId);

                    if (sdkCaseId != null)
                    {
                        OuterInnerResourceSite machineAreaSite = new OuterInnerResourceSite();
                        machineAreaSite.OuterInnerResourceId = machineArea.Id;
                        machineAreaSite.MicrotingSdkSiteId = siteDto.SiteId;
                        machineAreaSite.MicrotingSdkCaseId = (int)sdkCaseId;
                        machineAreaSite.MicrotingSdkeFormId = eFormId;
                        await machineAreaSite.Create(_dbContext);
                    }    
                }
            }

            var sitesConfigured = _dbContext.OuterInnerResourceSites.Where(x => x.OuterInnerResourceId == machineArea.Id).ToList();
            WriteLogEntry("MachineAreaUpdateHandler: sitesConfigured looked up");

            foreach (OuterInnerResourceSite machineAreaSite in sitesConfigured)
            {
                WriteLogEntry(
                    $"MachineAreaUpdateHandler: Looking at machineAreaSite {machineAreaSite.Id} for microtingSiteId {machineAreaSite.MicrotingSdkSiteId}");

                if (!siteIds.Contains(machineAreaSite.MicrotingSdkSiteId))
                {
                    WriteLogEntry($"MachineAreaUpdateHandler: {machineAreaSite.MicrotingSdkSiteId} not found in the list, so calling delete.");
                    
                    bool result = _core.CaseDelete(machineAreaSite.MicrotingSdkCaseId);
                    if (result)
                    {
                        await machineAreaSite.Delete(_dbContext);
                    }
                }
            }
        }

        private async Task DeleteRelationship(int machineAreaId, int microtingSdkCaseId)
        {
            OuterInnerResourceSite machineAreaSite =  
                _dbContext.OuterInnerResourceSites.SingleOrDefault(x => x.OuterInnerResourceId == machineAreaId && x.MicrotingSdkCaseId == microtingSdkCaseId);
            if (machineAreaSite != null)
            {
                bool result = _core.CaseDelete(microtingSdkCaseId);
                if (result)
                {
                    await machineAreaSite.Delete(_dbContext);
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