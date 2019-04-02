using System;
using System.Collections.Generic;
using System.Linq;
using eFormShared;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

namespace MachineArea.Pn.Infrastructure.Models
{
    public class MachineAreaSettingsModel
    {
        public List<MachineAreaSettingModel> machineAreaSettingsList { get; set; }

        public static bool SettingCreateDefaults(MachineAreaPnDbContext _dbcontext)
        {
            SettingCreate(_dbcontext, Settings.SdkConnectionString);
            SettingCreate(_dbcontext, Settings.LogLevel);
            SettingCreate(_dbcontext, Settings.LogLimit);
            SettingCreate(_dbcontext, Settings.MaxParallelism);
            SettingCreate(_dbcontext, Settings.NumberOfWorkers);
            SettingCreate(_dbcontext, Settings.Token);
            SettingCreate(_dbcontext, Settings.SdkeFormId);
            SettingCreate(_dbcontext, Settings.EnabledSiteIds);

            return true;
        }
        
        public static void SettingCreate(MachineAreaPnDbContext dbContext, Settings name)
        {
            #region id = settings.name
            int id = -1;
            string defaultValue = "default";
            switch (name)
            {
                case Settings.SdkConnectionString: defaultValue = "..."; break;
                case Settings.LogLevel: defaultValue = "4"; break;
                case Settings.LogLimit: defaultValue = "25000"; break;
                case Settings.MaxParallelism: defaultValue = "1"; break;
                case Settings.NumberOfWorkers: defaultValue = "1"; break;
                case Settings.Token: defaultValue = "..."; break;
                case Settings.SdkeFormId: defaultValue = "..."; break;
                case Settings.EnabledSiteIds: defaultValue = ""; break;
                
                default:
                    throw new IndexOutOfRangeException(name.ToString() + " is not a known/mapped Settings type");
            }
            #endregion

            if (dbContext.MachineAreaSettings.Count(x => x.Name == name.ToString()) < 1)
            {
                MachineAreaSettingModel machineAreaSettingModel = new MachineAreaSettingModel();
                machineAreaSettingModel.Name = name.ToString();
                machineAreaSettingModel.Value = defaultValue;
                machineAreaSettingModel.Save(dbContext);                
            }
        }      
        

        public string SettingRead(MachineAreaPnDbContext dbContext, Settings name)
        {
            MachineAreaSetting match = dbContext.MachineAreaSettings.Single(x => x.Name == name.ToString());

            if (match.Value == null)
                return "";

            return match.Value;
        }
        
        public List<string> SettingCheckAll(MachineAreaPnDbContext dbContext)
        {
            List<string> result = new List<string>();

                int countVal = dbContext.MachineAreaSettings.Count(x => x.Value == "");
                int countSet = dbContext.MachineAreaSettings.Count();

                if (countSet == 0)
                {
                    result.Add("NO SETTINGS PRESENT, NEEDS PRIMING!");
                    return result;
                }

                foreach (var setting in Enum.GetValues(typeof(Settings)))
                {
                    try
                    {
                        string readSetting = SettingRead(dbContext, (Settings)setting);
                        if (string.IsNullOrEmpty(readSetting))
                            result.Add(setting.ToString() + " has an empty value!");
                    }
                    catch
                    {
                        result.Add("There is no setting for " + setting + "! You need to add one");
                    }
                }
                return result;
        }
        
        public enum Settings
        {
            LogLevel,
            LogLimit,
            SdkConnectionString,
            MaxParallelism,
            NumberOfWorkers,
            Token,
            SdkeFormId,
            EnabledSiteIds
        }
    }
}
