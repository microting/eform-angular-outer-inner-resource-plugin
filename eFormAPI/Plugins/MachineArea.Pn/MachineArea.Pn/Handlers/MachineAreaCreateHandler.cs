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
using System.Configuration;
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
            if (message.MachineModel != null)
            {
                await CreateFromMachine(message.MachineModel);
            }
            else
            {
                await CreateFromArea(message.AreaModel);
            }            
        }

        private async Task CreateFromMachine(MachineModel model)
        {
            string eFormId = _dbContext.MachineAreaSettings
                .FirstOrDefault(x => 
                    x.Name == MachineAreaSettingsEnum.SdkeFormId.ToString())?.Value;

            MainElement mainElement = _core.TemplateRead(int.Parse(eFormId));
            List<Site_Dto> sites = new List<Site_Dto>();

            string sdkSiteIds = _dbContext.MachineAreaSettings
                .FirstOrDefault(x => 
                    x.Name == MachineAreaSettingsEnum.EnabledSiteIds.ToString())?.Value;
            foreach (string siteId in sdkSiteIds.Split(","))
            {
                sites.Add(_core.SiteRead(int.Parse(siteId)));
            }
            
            foreach (int areaId in model.RelatedAreasIds)
            {                
                Area area = _dbContext.Areas.SingleOrDefault(x => x.Id == areaId);
                await CreateRelationships(model.Id, areaId, model.Name, area.Name, mainElement, sites, int.Parse(eFormId));              
            }
        }

        private async Task CreateFromArea(AreaModel model)
        {
            string eFormId = _dbContext.MachineAreaSettings
                .FirstOrDefault(x => 
                    x.Name == MachineAreaSettingsEnum.SdkeFormId.ToString())?.Value;

            MainElement mainElement = _core.TemplateRead(int.Parse(eFormId));
            List<Site_Dto> sites = new List<Site_Dto>();
            
            string sdkSiteIds = _dbContext.MachineAreaSettings
                .FirstOrDefault(x => 
                    x.Name == MachineAreaSettingsEnum.EnabledSiteIds.ToString())?.Value;
            foreach (string siteId in sdkSiteIds.Split(","))
            {
                sites.Add(_core.SiteRead(int.Parse(siteId)));
            }
            
            foreach (int machineId in model.RelatedMachinesIds)
            {
                Machine machine = _dbContext.Machines.SingleOrDefault(x => x.Id == machineId);
                await CreateRelationships(machineId, model.Id, machine.Name, model.Name, mainElement, sites, int.Parse(eFormId));
            }
        }

        private async Task CreateRelationships(int machineId, int areaId, string machineName, string areaName, MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
            var match = await _dbContext.MachineAreas.SingleOrDefaultAsync(x =>
                    x.MachineId == machineId && x.AreaId == areaId);
                if (match == null)
                {
                    MachineAreaModel machineArea =
                        new MachineAreaModel();
                    machineArea.AreaId = areaId;
                    machineArea.MachineId = machineId;
                    await machineArea.Save(_dbContext);
                    mainElement.Label = machineName;
                    mainElement.ElementList[0].Label = machineName;
                    
                    mainElement.EnableQuickSync = true;
                    mainElement.CheckListFolderName = areaName;
                
                    foreach (Site_Dto siteDto in sites)
                    {
                        var siteMatch = await _dbContext.MachineAreaSites.SingleOrDefaultAsync(x =>
                            x.MicrotingSdkSiteId == siteDto.SiteId && x.MachineAreaId == machineArea.Id);
                        if (siteMatch == null)
                        {
                            string sdkCaseId = _core.CaseCreate(mainElement, "", siteDto.SiteId);

                            if (!string.IsNullOrEmpty(sdkCaseId))
                            {
                                MachineAreaSiteModel machineAreaSiteModel = new MachineAreaSiteModel();
                                machineAreaSiteModel.MachineAreaId = machineArea.Id;
                                machineAreaSiteModel.MicrotingSdkSiteId = siteDto.SiteId;
                                machineAreaSiteModel.MicrotingSdkCaseId = int.Parse(sdkCaseId);
                                machineAreaSiteModel.MicrotingSdkeFormId = eFormId;
                                await machineAreaSiteModel.Save(_dbContext);
                            }    
                        }
                    }    
                }     
        }
    }
}