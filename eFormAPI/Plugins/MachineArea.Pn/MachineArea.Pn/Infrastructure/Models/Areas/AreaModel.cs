using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eFormShared;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

namespace MachineArea.Pn.Infrastructure.Models.Areas
{
    public class AreaModel : IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RelatedMachinesIds { get; set; }

        public async Task Save(MachineAreaPnDbContext _dbContext)
        {
            Area area = new Area();
            area.Name = Name;
            area.CreatedAt = DateTime.Now;
            area.UpdatedAt = DateTime.Now;
            area.Version = 1;
            area.WorkflowState = Constants.WorkflowStates.Created;

            _dbContext.Areas.Add(area);
            _dbContext.SaveChanges();

            _dbContext.AreaVersions.Add(MapAreaVersion(_dbContext, area));
            _dbContext.SaveChanges();
            Id = area.Id;
        }

        public async Task Update(MachineAreaPnDbContext _dbContext)
        {
            Area area = _dbContext.Areas.FirstOrDefault(x => x.Id == Id);

            if (area == null)
            {
                throw new NullReferenceException($"Could not find area with id: {Id}");
            }

            area.Name = Name;

            if (_dbContext.ChangeTracker.HasChanges())
            {
                area.UpdatedAt = DateTime.Now;
                area.Version += 1;

                _dbContext.AreaVersions.Add(MapAreaVersion(_dbContext, area));
                _dbContext.SaveChanges();
            }
        }

        public async Task Delete(MachineAreaPnDbContext _dbContext)
        {
            Area area = _dbContext.Areas.FirstOrDefault(x => x.Id == Id);

            if (area == null)
            {
                throw new NullReferenceException($"Could not find area with id: {Id}");
            }

            area.WorkflowState = Constants.WorkflowStates.Removed;

            if (_dbContext.ChangeTracker.HasChanges())
            {
                area.UpdatedAt = DateTime.Now;
                area.Version += 1;

                _dbContext.AreaVersions.Add(MapAreaVersion(_dbContext, area));
                _dbContext.SaveChanges();
            }
        }

        private AreaVersion MapAreaVersion(MachineAreaPnDbContext _dbContext, Area area)
        {
            AreaVersion areaVer = new AreaVersion();

            areaVer.Name = area.Name; 
            areaVer.Version = area.Version;
            areaVer.AreaId = area.Id;
            areaVer.CreatedAt = area.CreatedAt;
            areaVer.UpdatedAt = area.UpdatedAt;


            return areaVer;
        }
    }
}
