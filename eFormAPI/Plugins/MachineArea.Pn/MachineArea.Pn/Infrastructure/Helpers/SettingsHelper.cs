using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MachineArea.Pn.Infrastructure.Models;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Consts;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

namespace MachineArea.Pn.Infrastructure.Helpers
{
    public static class SettingsHelper
    {
        public static bool SettingCreateDefaults(MachineAreaPnDbContext _dbcontext)
        {
            SettingCreate(_dbcontext, MachineAreaSettingsEnum.SdkConnectionString);
            SettingCreate(_dbcontext, MachineAreaSettingsEnum.LogLevel);
            SettingCreate(_dbcontext, MachineAreaSettingsEnum.LogLimit);
            SettingCreate(_dbcontext, MachineAreaSettingsEnum.MaxParallelism);
            SettingCreate(_dbcontext, MachineAreaSettingsEnum.NumberOfWorkers);
            SettingCreate(_dbcontext, MachineAreaSettingsEnum.Token);
            SettingCreate(_dbcontext, MachineAreaSettingsEnum.SdkeFormId);
            SettingCreate(_dbcontext, MachineAreaSettingsEnum.EnabledSiteIds);

            return true;
        }

        public static void SettingCreate(MachineAreaPnDbContext dbContext, MachineAreaSettingsEnum name)
        {
            #region id = settings.name
            int id = -1;
            string defaultValue = "default";
            switch (name)
            {
                case MachineAreaSettingsEnum.SdkConnectionString: defaultValue = "..."; break;
                case MachineAreaSettingsEnum.LogLevel: defaultValue = "4"; break;
                case MachineAreaSettingsEnum.LogLimit: defaultValue = "25000"; break;
                case MachineAreaSettingsEnum.MaxParallelism: defaultValue = "1"; break;
                case MachineAreaSettingsEnum.NumberOfWorkers: defaultValue = "1"; break;
                case MachineAreaSettingsEnum.Token: defaultValue = "..."; break;
                case MachineAreaSettingsEnum.SdkeFormId: defaultValue = "..."; break;
                case MachineAreaSettingsEnum.EnabledSiteIds: defaultValue = ""; break;

                default:
                    throw new IndexOutOfRangeException(name.ToString() + " is not a known/mapped Settings type");
            }
            #endregion

            if (dbContext.MachineAreaSettings.Count(x => x.Name == name.ToString()) < 1)
            {
                MachineAreaSetting newSettings = new MachineAreaSetting
                {
                    Name = name.ToString(),
                    Value = defaultValue
                };

                newSettings.Save(dbContext);
            }
        }
    }
}
