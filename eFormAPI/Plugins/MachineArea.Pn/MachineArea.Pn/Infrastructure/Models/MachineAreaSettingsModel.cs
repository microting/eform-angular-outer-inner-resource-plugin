using System;
using System.Linq;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

namespace MachineArea.Pn.Infrastructure.Models
{
    public class MachineAreaSettingsModel
    {
        public int? SelectedTemplateId { get; set; }
        public string SelectedTemplateName { get; set; }

        public void Save(MachineAreaPnDbContext _dbContext)
        {
            MachineAreaSetting customerSettings = new MachineAreaSetting();

            customerSettings.SelectedeFormId = SelectedTemplateId;
            customerSettings.SelectedeFormName = SelectedTemplateName;

            _dbContext.MachineAreaSettings.Add(customerSettings);
            _dbContext.SaveChanges();
        }

        public void Update(MachineAreaPnDbContext _dbContext)
        {
            MachineAreaSetting customerSettings = _dbContext.MachineAreaSettings.FirstOrDefault();

            if (customerSettings == null)
            {
                throw new ArgumentNullException($"Could not find Setting with id {SelectedTemplateId}");
            }

            customerSettings.SelectedeFormId = SelectedTemplateId;
            customerSettings.SelectedeFormName = SelectedTemplateName;

            if (_dbContext.ChangeTracker.HasChanges())
            {
                _dbContext.SaveChanges();
            }
        }
    }
}
