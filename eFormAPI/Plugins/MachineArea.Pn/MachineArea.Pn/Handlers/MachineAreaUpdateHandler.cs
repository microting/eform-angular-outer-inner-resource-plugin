/*
The MIT License (MIT)

Copyright (c) 2007 - 2019 microting

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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eFormCore;
using eFormData;
using eFormShared;
using MachineArea.Pn.Infrastructure.Models;
using MachineArea.Pn.Infrastructure.Models.Areas;
using MachineArea.Pn.Infrastructure.Models.Machines;
using MachineArea.Pn.Messages;
using Microsoft.EntityFrameworkCore;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Consts;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;
using Rebus.Handlers;

namespace MachineArea.Pn.Handlers
{
    public class MachineAreaUpdateHandler : IHandleMessages<MachineAreaUpdate>
    {  
        private readonly Core _core;
        private readonly MachineAreaPnDbContext _dbContext;        
        
        public MachineAreaUpdateHandler(Core core, MachineAreaPnDbContext context)
        {
            _core = core;
            _dbContext = context;
        }
        
        #pragma warning disable 1998
        public async Task Handle(MachineAreaUpdate message)
        {            
            string lookup = $"MachineAreaBaseSettings:{MachineAreaSettingsEnum.SdkeFormId.ToString()}"; 
            
            int eFormId = int.Parse(_dbContext.PluginConfigurationValues
                .FirstOrDefault(x => 
                    x.Name == lookup)?.Value);

            MainElement mainElement = _core.TemplateRead(eFormId);
            List<Site_Dto> sites = new List<Site_Dto>();
            
            lookup = $"MachineAreaBaseSettings:{MachineAreaSettingsEnum.EnabledSiteIds.ToString()}"; 
            string sdkSiteIds = _dbContext.PluginConfigurationValues
                .FirstOrDefault(x => 
                    x.Name == lookup)?.Value;
            foreach (string siteId in sdkSiteIds.Split(","))
            {
                sites.Add(_core.SiteRead(int.Parse(siteId)));
            }
            
            if (message.MachineModel != null)
            {
                await UpdateFromMachine(message.MachineModel, mainElement, sites, eFormId);
            }
            else
            {
                await UpdateFromArea(message.AreaModel, mainElement, sites, eFormId);
            }
        }

        private async Task UpdateFromMachine(MachineModel machineModel, MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
            List<Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea> machineAreas = _dbContext.MachineAreas.Where(x =>
                x.MachineId == machineModel.Id && x.WorkflowState != Constants.WorkflowStates.Removed).ToList();

            List<int> requestedAreaIds = machineModel.RelatedAreasIds;
            List<int> deployedAreaIds = new List<int>();
            List<int> toBedeployed = new List<int>();
            
            foreach (var machineArea in machineAreas)
            {                
                deployedAreaIds.Add(machineArea.AreaId);

                if (!machineModel.RelatedAreasIds.Contains(machineArea.AreaId))
                {
                    foreach (MachineAreaSite machineAreaSite in machineArea.MachineAreaSites)
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
                Area area = _dbContext.Areas.SingleOrDefault(x => x.Id == areaId);
                if (area != null)
                    await CreateRelationships(machineModel.Id, areaId, machineModel.Name, area.Name, mainElement, sites,
                        eFormId);
            }
        }

        private async Task UpdateFromArea(AreaModel areaModel, MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
            List<Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea> machineAreas = _dbContext.MachineAreas.Where(x =>
                x.AreaId == areaModel.Id && x.WorkflowState != Constants.WorkflowStates.Removed).ToList();

            List<int> requestedMachineIds = areaModel.RelatedMachinesIds;
            List<int> deployedMachineIds = new List<int>();
            List<int> toBedeployed = new List<int>();
            
            foreach (var machineArea in machineAreas)
            {
                deployedMachineIds.Add(machineArea.MachineId);
                
                if (!areaModel.RelatedMachinesIds.Contains(machineArea.MachineId))
                {
                    foreach (MachineAreaSite machineAreaSite in machineArea.MachineAreaSites)
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
                Machine machine = _dbContext.Machines.SingleOrDefault(x => x.Id == machineId);
                if (machine != null)
                    await CreateRelationships(machineId, areaModel.Id, machine.Name, areaModel.Name, mainElement, sites,
                        eFormId);
            }
        }

        private async Task CreateRelationships(int machineId, int areaId, string machineName, string areaName, MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
            var machineArea = _dbContext.MachineAreas.SingleOrDefault(x =>
                    x.MachineId == machineId && x.AreaId == areaId);

            if (machineArea != null)
            {
                machineArea.WorkflowState = Constants.WorkflowStates.Created;
                await machineArea.Save(_dbContext);
            }
            else
            {
                machineArea =
                    new Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea();
                machineArea.AreaId = areaId;
                machineArea.MachineId = machineId;
                await machineArea.Save(_dbContext);
            }
            
            mainElement.Label = machineName;
            mainElement.ElementList[0].Label = machineName;
            
            mainElement.EnableQuickSync = true;
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
            
            foreach (Site_Dto siteDto in sites)
            {
                MachineAreaSite siteMatch = _dbContext.MachineAreaSites.SingleOrDefault(x =>
                    x.MicrotingSdkSiteId == siteDto.SiteId && x.MachineAreaId == machineArea.Id && x.WorkflowState == Constants.WorkflowStates.Created);
                if (siteMatch == null)
                {
                    string sdkCaseId = _core.CaseCreate(mainElement, "", siteDto.SiteId);

                    if (!string.IsNullOrEmpty(sdkCaseId))
                    {
                        MachineAreaSite machineAreaSite = new MachineAreaSite();
                        machineAreaSite.MachineAreaId = machineArea.Id;
                        machineAreaSite.MicrotingSdkSiteId = siteDto.SiteId;
                        machineAreaSite.MicrotingSdkCaseId = int.Parse(sdkCaseId);
                        machineAreaSite.MicrotingSdkeFormId = eFormId;
                        await machineAreaSite.Save(_dbContext);
                    }    
                }
            }    
        }

        private async Task DeleteRelationship(int machineAreaId, int microtingSdkCaseId)
        {
            MachineAreaSite machineAreaSite =  
                _dbContext.MachineAreaSites.SingleOrDefault(x => x.MachineAreaId == machineAreaId && x.MicrotingSdkCaseId == microtingSdkCaseId);
            if (machineAreaSite != null)
            {
                bool result = _core.CaseDelete(microtingSdkCaseId.ToString());
                if (result)
                {
                    await machineAreaSite.Delete(_dbContext);
                }
            }
        }
    }
}