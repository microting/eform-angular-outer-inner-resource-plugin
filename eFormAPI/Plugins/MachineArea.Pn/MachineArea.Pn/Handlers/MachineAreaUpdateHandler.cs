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
using MachineArea.Pn.Infrastructure.Models.Areas;
using MachineArea.Pn.Infrastructure.Models.Machines;
using MachineArea.Pn.Messages;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
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
            if (message.MachineModel != null)
            {
                await UpdateFromMachine(message.MachineModel);
            }
            else
            {
                await UpdateFromArea(message.AreaModel);
            }
        }

        private async Task UpdateFromMachine(MachineModel machineModel)
        {
            List<Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea> machineAreas = _dbContext.MachineAreas.Where(x =>
                x.MachineId == machineModel.Id).ToList();
        }

        private async Task UpdateFromArea(AreaModel areaModel)
        {
            List<Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea> machineAreas = _dbContext.MachineAreas.Where(x =>
                x.AreaId == areaModel.Id).ToList();
        }

        private async Task UpdateRelationships()
        {
            
        }
    }
}