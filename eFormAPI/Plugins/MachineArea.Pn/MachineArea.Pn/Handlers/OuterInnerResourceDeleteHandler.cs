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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eFormCore;
using MachineArea.Pn.Infrastructure.Models;
using MachineArea.Pn.Infrastructure.Models.Areas;
using MachineArea.Pn.Infrastructure.Models.Machines;
using MachineArea.Pn.Messages;
using Microsoft.EntityFrameworkCore;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using Rebus.Handlers;

namespace MachineArea.Pn.Handlers
{
    public class OuterInnerResourceDeleteHandler : IHandleMessages<OuterInnerResourceDelete>
    {    
        private readonly Core _core;
        private readonly OuterInnerResourcePnDbContext _dbContext;        
        
        public OuterInnerResourceDeleteHandler(Core core, OuterInnerResourcePnDbContext context)
        {
            _core = core;
            _dbContext = context;
        }
        
        #pragma warning disable 1998
        public async Task Handle(OuterInnerResourceDelete message)
        {            
            if (message.MachineModel != null)
            {
                await DeleteFromMachine(message.MachineModel);
            }
            else
            {
                await DeleteFromArea(message.AreaModel);
            }     
        }

        private async Task DeleteFromMachine(MachineModel machineModel)
        {
            List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> machineAreas = _dbContext.OuterInnerResources.Where(x =>
                x.InnerResourceId == machineModel.Id).ToList();
            await DeleteRelationships(machineAreas);
        }

        private async Task DeleteFromArea(AreaModel areaModel)
        {
            List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> machineAreas = _dbContext.OuterInnerResources.Where(x =>
                x.OuterResourceId == areaModel.Id).ToList();
            await DeleteRelationships(machineAreas);

        }

        private async Task DeleteRelationships(List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> machineAreas)
        {
            foreach (Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource machineArea in machineAreas)
            {
                IQueryable<OuterInnerResourceSite> machineAreaSites = _dbContext.OuterInnerResourceSites.Where(x => x.OuterInnerResourceId == machineArea.Id);
                int numSites = machineAreaSites.Count();
                int sitesDeleted = 0;
                foreach (OuterInnerResourceSite machineAreaSite in machineAreaSites)
                {
                    try
                    {
                        bool result = _core.CaseDelete(machineAreaSite.MicrotingSdkCaseId);
                        if (result)
                        {
                            await machineAreaSite.Delete(_dbContext);
                            sitesDeleted += 1;
                        }
                    }
                    catch
                    {
                        
                    }
                    
                }

                if (numSites == sitesDeleted)
                {
                    await machineArea.Delete(_dbContext);
                }
            }
        }
    }
}