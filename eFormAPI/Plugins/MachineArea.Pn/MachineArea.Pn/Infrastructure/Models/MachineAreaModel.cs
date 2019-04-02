using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eFormShared;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

namespace MachineArea.Pn.Infrastructure.Models
{
    public class MachineAreaModel : IModel
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public int AreaId { get; set; }
        public string WorkflowState { get; set; }
        
        public async Task Save(MachineAreaPnDbContext _dbContext)
        {            
            Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea machineArea = new 
                Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea();
            machineArea.MachineId = MachineId;
            machineArea.AreaId = AreaId;
            machineArea.CreatedAt = DateTime.Now;
            machineArea.UpdatedAt = DateTime.Now;
            machineArea.Version = 1;
            machineArea.WorkflowState = Constants.WorkflowStates.Created;

            _dbContext.MachineAreas.Add(machineArea);
            _dbContext.SaveChanges();

            _dbContext.MachineAreaVersions.Add(MapMachineAreaVersion(_dbContext, machineArea));
            _dbContext.SaveChanges();

            Id = machineArea.Id;
        }

        public async Task Update(MachineAreaPnDbContext _dbContext)
        {
            
            Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea machineArea = 
                _dbContext.MachineAreas.FirstOrDefault(x => x.Id == Id);

            if (machineArea == null)
            {
                throw new NullReferenceException($"Could not find area with id: {Id}");
            }

            machineArea.WorkflowState = WorkflowState;
            
            if (_dbContext.ChangeTracker.HasChanges())
            {
                machineArea.UpdatedAt = DateTime.Now;
                machineArea.Version += 1;

                _dbContext.MachineAreaVersions.Add(MapMachineAreaVersion(_dbContext, machineArea));
                _dbContext.SaveChanges();
            }
        }

        public async Task Delete(MachineAreaPnDbContext _dbContext)
        {

            Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea machineArea = 
                _dbContext.MachineAreas.FirstOrDefault(x => x.Id == Id);

            if (machineArea == null)
            {
                throw new NullReferenceException($"Could not find area with id: {Id}");
            }

            machineArea.WorkflowState = Constants.WorkflowStates.Removed;

            if (_dbContext.ChangeTracker.HasChanges())
            {
                machineArea.UpdatedAt = DateTime.Now;
                machineArea.Version += 1;

                _dbContext.MachineAreaVersions.Add(MapMachineAreaVersion(_dbContext, machineArea));
                _dbContext.SaveChanges();
            }        
        }

        private MachineAreaVersion MapMachineAreaVersion(MachineAreaPnDbContext _dbContext,
            Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea machineArea)
        {
            MachineAreaVersion machineAreaVersion = new MachineAreaVersion();

            machineAreaVersion.MachineId = machineArea.MachineId; 
            machineAreaVersion.Version = machineArea.Version;
            machineAreaVersion.AreaId = machineArea.AreaId;
            machineAreaVersion.MachineAreaId = machineArea.Id;


            return machineAreaVersion;
        }
    }
}