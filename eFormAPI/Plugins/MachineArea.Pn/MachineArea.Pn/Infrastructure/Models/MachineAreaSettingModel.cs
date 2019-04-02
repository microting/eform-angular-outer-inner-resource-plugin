using System;
using System.ComponentModel.DataAnnotations;
using eFormShared;
using System.Linq;
using System.Threading.Tasks;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

namespace MachineArea.Pn.Infrastructure.Models
{
    public class MachineAreaSettingModel : IModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [StringLength(255)] public string WorkflowState { get; set; }
        public int Version { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public string Name { get; set; }


        public async Task Save(MachineAreaPnDbContext dbContext)
        {

            MachineAreaSetting machineAreaSetting = new MachineAreaSetting();
            if (CreatedAt != null)
            {
                machineAreaSetting.CreatedAt = (DateTime) CreatedAt;
            }

            machineAreaSetting.CreatedByUserId = CreatedByUserId;
            machineAreaSetting.UpdatedByUserId = UpdatedByUserId;
            machineAreaSetting.CreatedAt = DateTime.Now;
            machineAreaSetting.UpdatedAt = DateTime.Now;
            machineAreaSetting.Name = Name;
            machineAreaSetting.Value = Value;
            machineAreaSetting.Version = 1;
            machineAreaSetting.WorkflowState = Constants.WorkflowStates.Created;


            dbContext.MachineAreaSettings.Add(machineAreaSetting);
            dbContext.SaveChanges();

            dbContext.MachineAreaSettingVersions.Add(MapMachineAreaSettingVersion(dbContext, machineAreaSetting));
            dbContext.SaveChanges();
        }

        public async Task Update(MachineAreaPnDbContext dbContext)
        {

            MachineAreaSetting machineAreaSetting = dbContext.MachineAreaSettings.FirstOrDefault(x => x.Name == Name);

            if (machineAreaSetting == null)
            {
                throw new NullReferenceException($"Could not find TrashInspectionPnSettings with Name: {Name}");
            }

            machineAreaSetting.Value = Value;

            if (dbContext.ChangeTracker.HasChanges())
            {
                machineAreaSetting.UpdatedAt = DateTime.Now;
                machineAreaSetting.UpdatedByUserId = UpdatedByUserId;
                machineAreaSetting.Version += 1;

                dbContext.MachineAreaSettingVersions.Add(MapMachineAreaSettingVersion(dbContext, machineAreaSetting));
                dbContext.SaveChanges();
            }

        }

        public async Task Delete(MachineAreaPnDbContext dbContext)
        {

            MachineAreaSetting machineAreaSetting = dbContext.MachineAreaSettings.FirstOrDefault(x => x.Name == Name);

            if (machineAreaSetting == null)
            {
                throw new NullReferenceException($"Could not find trashInspectionPnSetting with Name: {Name}");
            }

            machineAreaSetting.WorkflowState = Constants.WorkflowStates.Removed;

            if (dbContext.ChangeTracker.HasChanges())
            {
                machineAreaSetting.UpdatedAt = DateTime.Now;
                machineAreaSetting.UpdatedByUserId = UpdatedByUserId;
                machineAreaSetting.Version += 1;
                dbContext.MachineAreaSettingVersions.Add(MapMachineAreaSettingVersion(dbContext, machineAreaSetting));
                dbContext.SaveChanges();
            }

        }

        private MachineAreaSettingVersion MapMachineAreaSettingVersion(MachineAreaPnDbContext dbContext,
            MachineAreaSetting machineAreaSetting)
        {
            MachineAreaSettingVersion machineAreaSettingVersion = new MachineAreaSettingVersion();

            machineAreaSettingVersion.CreatedAt = machineAreaSetting.CreatedAt;
            machineAreaSettingVersion.CreatedByUserId = machineAreaSetting.CreatedByUserId;
            machineAreaSettingVersion.Name = machineAreaSetting.Name;
            machineAreaSettingVersion.Value = machineAreaSetting.Value;
            machineAreaSettingVersion.UpdatedAt = machineAreaSetting.UpdatedAt;
            machineAreaSettingVersion.UpdatedByUserId = machineAreaSetting.UpdatedByUserId;
            machineAreaSettingVersion.Version = machineAreaSetting.Version;
            machineAreaSettingVersion.WorkflowState = machineAreaSetting.WorkflowState;

            machineAreaSettingVersion.MachineAreaSettingId = machineAreaSetting.Id;

            return machineAreaSettingVersion;
        }
    }
}