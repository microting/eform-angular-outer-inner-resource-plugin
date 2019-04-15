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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using eFormCore;
using eFormData;
using eFormShared;
using eFormSqlController;
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
    public class MachineAreaCreateHandler : IHandleMessages<MachineAreaCreate>
    {        
        private readonly Core _core;
        private readonly MachineAreaPnDbContext _dbContext;        
        
        public MachineAreaCreateHandler(Core core, MachineAreaPnDbContext context)
        {
            _core = core;
            _dbContext = context;
        }
        
        #pragma warning disable 1998
        public async Task Handle(MachineAreaCreate message)
        {            
            string lookup = $"MachineAreaBaseSettings:{MachineAreaSettingsEnum.SdkeFormId.ToString()}"; 
            
            LogEvent($"lookup is {lookup}");

            string result = _dbContext.PluginConfigurationValues.AsNoTracking()
                .FirstOrDefault(x =>
                    x.Name == lookup)
                ?.Value;
            
            LogEvent($"result is {result}");
            
            int eFormId = int.Parse(result);

            MainElement mainElement = _core.TemplateRead(eFormId);
            List<Site_Dto> sites = new List<Site_Dto>();
            
            lookup = $"MachineAreaBaseSettings:{MachineAreaSettingsEnum.EnabledSiteIds.ToString()}"; 
            LogEvent($"lookup is {lookup}");

            string sdkSiteIds = _dbContext.PluginConfigurationValues.AsNoTracking()
                .FirstOrDefault(x => 
                    x.Name == lookup)?.Value;
            
            LogEvent($"sdkSiteIds is {sdkSiteIds}");
            
            foreach (string siteId in sdkSiteIds.Split(","))
            {
                LogEvent($"found siteId {siteId}");
                sites.Add(_core.SiteRead(int.Parse(siteId)));
            }
            
            if (message.MachineModel != null)
            {
                await CreateFromMachine(message.MachineModel, mainElement, sites, eFormId);
            }
            else
            {
                await CreateFromArea(message.AreaModel, mainElement, sites, eFormId);
            }            
        }

        private async Task CreateFromMachine(MachineModel model, MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
            foreach (int areaId in model.RelatedAreasIds)
            {                
                Area area = _dbContext.Areas.SingleOrDefault(x => x.Id == areaId);
                await CreateRelationships(model.Id, areaId, model.Name, area.Name, mainElement, sites, eFormId);              
            }
        }

        private async Task CreateFromArea(AreaModel model, MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
           foreach (int machineId in model.RelatedMachinesIds)
            {
                Machine machine = _dbContext.Machines.SingleOrDefault(x => x.Id == machineId);
                await CreateRelationships(machineId, model.Id, machine.Name, model.Name, mainElement, sites, eFormId);
            }
        }

        private async Task CreateRelationships(int machineId, int areaId, string machineName, string areaName, MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
            var match = _dbContext.MachineAreas.SingleOrDefault(x =>
                    x.MachineId == machineId && x.AreaId == areaId);
            if (match == null)
            {
                Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea machineArea =
                    new Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea();
                machineArea.AreaId = areaId;
                machineArea.MachineId = machineId;
                await machineArea.Save(_dbContext);
                mainElement.Label = machineName;
                mainElement.ElementList[0].Label = machineName;
                mainElement.EndDate = DateTime.Now.AddYears(10).ToUniversalTime();
                mainElement.StartDate = DateTime.Now.ToUniversalTime();
                mainElement.Repeated = 0;
                
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
                    var siteMatch = _dbContext.MachineAreaSites.SingleOrDefault(x =>
                        x.MicrotingSdkSiteId == siteDto.SiteId && x.MachineAreaId == machineArea.Id);
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
        }
        
        private void LogEvent(string appendText)
        {
            try
            {                
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("[DBG] " + appendText);
                Console.ForegroundColor = oldColor;
            }
            catch
            {
            }
        }

        private void LogException(string appendText)
        {
            try
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERR] " + appendText);
                Console.ForegroundColor = oldColor;
            }
            catch
            {

            }
        }
    }
}