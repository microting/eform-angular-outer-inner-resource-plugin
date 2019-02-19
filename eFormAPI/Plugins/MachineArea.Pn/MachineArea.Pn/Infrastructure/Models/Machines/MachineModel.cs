using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eFormShared;
using Microsoft.CodeAnalysis;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

namespace MachineArea.Pn.Infrastructure.Models.Machines
{
    public class MachineModel : IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RelatedAreasIds { get; set; }

        public async Task Save(MachineAreaPnDbContext _dbContext)
        {
            Machine machine = new Machine();
            machine.Name = Name;
            
            _dbContext.Machines.Add(machine);
            _dbContext.SaveChanges();

            _dbContext.MachineVersions.Add(MapMachineVersion(_dbContext, machine));
            _dbContext.SaveChanges();
        }

        public async Task Update(MachineAreaPnDbContext _dbContext)
        {
            Machine machine = _dbContext.Machines.FirstOrDefault(x => x.Id == Id);

            if (machine == null)
            {
                throw new NullReferenceException($"Could not find Machine with id: {Id}");
            }

            machine.Name = Name;

            if (_dbContext.ChangeTracker.HasChanges())
            {
                machine.UpdatedAt = DateTime.Now;
                machine.Version += 1;

                _dbContext.MachineVersions.Add(MapMachineVersion(_dbContext, machine));
                _dbContext.SaveChanges();
            }
        }

        public async Task Delete(MachineAreaPnDbContext _dbContext)
        {
            Machine machine = _dbContext.Machines.FirstOrDefault(x => x.Id == Id);

            if (machine == null)
            {
                throw new NullReferenceException($"Could not find machine with id: {Id}");
            }

            machine.WorkflowState = Constants.WorkflowStates.Removed;

            if (_dbContext.ChangeTracker.HasChanges())
            {
                machine.UpdatedAt = DateTime.Now;
                machine.Version += 1;

                _dbContext.MachineVersions.Add(MapMachineVersion(_dbContext, machine));
                _dbContext.SaveChanges();
            }
        }

        private MachineVersion MapMachineVersion(MachineAreaPnDbContext _dbContext, Machine machine)
        {
            MachineVersion machineVer = new MachineVersion();

            machineVer.Name = machine.Name;
            machineVer.Machine = machine;
            machineVer.Version = machine.Version;
            machineVer.MachineId = machine.Id;
            
            
            return machineVer;
        }
    }
}
