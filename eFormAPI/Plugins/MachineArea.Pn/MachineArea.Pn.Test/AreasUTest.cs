using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using eFormShared;
using MachineArea.Pn.Infrastructure.Models.Areas;
using Microsoft.EntityFrameworkCore;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;
using NUnit.Framework;


namespace MachineArea.Pn.Test
{
    [TestFixture]
    public class AreasUTest : DbTestFixture
    {
        [Test]
        public async Task AreasModel_Save_DoesSave()
        {
           // Arrange
            Area newArea = new Area()
            {
                Name = Guid.NewGuid().ToString()
            };
            // Act
            await newArea.Save(DbContext);

            Area area = DbContext.Areas.AsNoTracking().First();
            List<Area> areaList = DbContext.Areas.AsNoTracking().ToList();
            List<AreaVersion> versionList = DbContext.AreaVersions.AsNoTracking().ToList();
            
            // Assert
            Assert.NotNull(area);
            Assert.AreEqual(1, areaList.Count());
            Assert.AreEqual(1, versionList.Count());
            
            Assert.AreEqual(newArea.Name, area.Name);
        }

        [Test]
        public async Task AreaModel_Update_DoesUpdate()
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

        [Test]
        public async Task AreaModel_Delete_DoesDelete()
        {
            //Arrange
            Area area = new Area
            {
                Name = Guid.NewGuid().ToString()
            };

            DbContext.Areas.Add(area);
            DbContext.SaveChanges();

            //Act
            Area selectedArea = new Area
            {
                Id = area.Id
            };

            await selectedArea.Delete(DbContext);
            
            Area dbArea = DbContext.Areas.AsNoTracking().First();
            List<Area> areaList = DbContext.Areas.AsNoTracking().ToList();
            List<AreaVersion> versionList = DbContext.AreaVersions.AsNoTracking().ToList();
            
            // Assert
            
            Assert.NotNull(dbArea);
            Assert.AreEqual(1, areaList.Count());
            Assert.AreEqual(1, versionList.Count());
            
            Assert.AreEqual(dbArea.WorkflowState, Constants.WorkflowStates.Removed);
        }
    }
}