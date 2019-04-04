using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eFormShared;
using MachineArea.Pn.Messages;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

namespace MachineArea.Pn.Infrastructure.Models
{
    public class MachineAreaSiteModel : IModel
    {

        public int Id { get; set; }
        public int MachineAreaId { get; set; }
        public int MicrotingSdkSiteId { get; set; }
        public int MicrotingSdkCaseId { get; set; }
        public int MicrotingSdkeFormId { get; set; }
        public string WorkflowState { get; set; }

        public async Task Save(MachineAreaPnDbContext _dbContext)
        {            
            MachineAreaSite machineAreaSite = new 
                MachineAreaSite();
            machineAreaSite.MachineAreaId = MachineAreaId;
            machineAreaSite.MicrotingSdkSiteId = MicrotingSdkSiteId;
            machineAreaSite.MicrotingSdkCaseId = MicrotingSdkCaseId;
            machineAreaSite.MicrotingSdkeFormId = MicrotingSdkeFormId;
            machineAreaSite.CreatedAt = DateTime.Now;
            machineAreaSite.UpdatedAt = DateTime.Now;
            machineAreaSite.Version = 1;
            machineAreaSite.WorkflowState = eFormShared.Constants.WorkflowStates.Created;

            _dbContext.MachineAreaSites.Add(machineAreaSite);
            _dbContext.SaveChanges();

            _dbContext.MachineAreaSiteVersions.Add(MapMachineAreaSiteVersion(_dbContext, machineAreaSite));
            _dbContext.SaveChanges();

            Id = machineAreaSite.Id;
        }

        public async Task Update(MachineAreaPnDbContext _dbContext)
        {                        
            MachineAreaSite machineAreaSite = 
                _dbContext.MachineAreaSites.FirstOrDefault(x => x.Id == Id);

            if (machineAreaSite == null)
            {
                throw new NullReferenceException($"Could not find area with id: {Id}");
            }

            machineAreaSite.WorkflowState = WorkflowState;
            
            if (_dbContext.ChangeTracker.HasChanges())
            {
                machineAreaSite.UpdatedAt = DateTime.Now;
                machineAreaSite.Version += 1;

                _dbContext.MachineAreaSiteVersions.Add(MapMachineAreaSiteVersion(_dbContext, machineAreaSite));
                _dbContext.SaveChanges();
            }
        }

        public async Task Delete(MachineAreaPnDbContext _dbContext)
        {
            
            MachineAreaSite machineAreaSite = _dbContext.MachineAreaSites.FirstOrDefault(x => x.Id == Id);

            if (machineAreaSite == null)
            {
                throw new NullReferenceException($"Could not find area with id: {Id}");
            }

            machineAreaSite.WorkflowState = Constants.WorkflowStates.Removed;

            if (_dbContext.ChangeTracker.HasChanges())
            {
                machineAreaSite.UpdatedAt = DateTime.Now;
                machineAreaSite.Version += 1;

                _dbContext.MachineAreaSiteVersions.Add(MapMachineAreaSiteVersion(_dbContext, machineAreaSite));
                _dbContext.SaveChanges();
            }        
        }

        private MachineAreaSiteVersion MapMachineAreaSiteVersion(MachineAreaPnDbContext _dbContext, 
            MachineAreaSite machineAreaSite)
        {
            MachineAreaSiteVersion machineAreaVersion = new MachineAreaSiteVersion();

            machineAreaVersion.MachineAreaId = machineAreaSite.MachineAreaId; 
            machineAreaVersion.Version = machineAreaSite.Version;
            machineAreaVersion.MicrotingSdkeFormId = machineAreaSite.MicrotingSdkeFormId;
            machineAreaVersion.MicrotingSdkSiteId = machineAreaSite.MicrotingSdkSiteId;
            machineAreaVersion.MachineAreaSiteId = machineAreaSite.Id;


            return machineAreaVersion;
        }
    }
}