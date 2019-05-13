using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using eFormShared;
using MachineArea.Pn.Infrastructure.Models.Areas;
using MachineArea.Pn.Infrastructure.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;
using NUnit.Framework;


namespace MachineArea.Pn.Test
{
    [TestFixture]
    public class SettingsUTest : DbTestFixture
    {
        [Test]
        public async Task Settings_Save_DoesSave()
        {
            // Arrange
            MachineAreaBaseSettings machineAreaSettings = new MachineAreaBaseSettings()
            {
                EnabledSiteIds = "EnabledSiteIds",
                LogLevel = "LogLevel",
                LogLimit = "LogLimit",
                MaxParallelism = "MaxParallelism",
                NumberOfWorkers = 1,
                QuickSyncEnabled = true,
                SdkConnectionString = "SdkConnectionString",
                SdkeFormId = "SdkeFormId",
                Token = "Token"
            };

            
            // Act
            // await DbContext.Ma.Save(DbContext);

            Area area = DbContext.Areas.AsNoTracking().First();
            List<Area> areaList = DbContext.Areas.AsNoTracking().ToList();
            List<AreaVersion> versionList = DbContext.AreaVersions.AsNoTracking().ToList();

            // Assert
            

            // Assert.AreEqual(newArea.Name, area.Name);
        }

        [Test]
        public async Task Settings_Update_DoesUpdate()
        {
            // Arrange
            Area area = new Area
            {
                Name = Guid.NewGuid().ToString()
            };

            DbContext.Areas.Add(area);
            DbContext.SaveChanges();

            //Act
            Area selectedArea = new Area
            {
                Name = Guid.NewGuid().ToString(),
                Id = area.Id
            };

            await selectedArea.Update(DbContext);

            Area dbArea = DbContext.Areas.AsNoTracking().First();
            List<Area> areaList = DbContext.Areas.AsNoTracking().ToList();
            List<AreaVersion> versionList = DbContext.AreaVersions.AsNoTracking().ToList();

            //Assert

            Assert.NotNull(dbArea);
            Assert.AreEqual(1, areaList.Count());
            Assert.AreEqual(1, versionList.Count());

            Assert.AreEqual(selectedArea.Name, dbArea.Name);
        }
    }
}