using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using eFormShared;
using MachineArea.Pn.Infrastructure.Models.Machines;
using Microsoft.EntityFrameworkCore;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;
using NUnit.Framework;
using MachineArea = Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea;

namespace MachineArea.Pn.Test
{
    [TestFixture]
    public class MachinesUTest : DbTestFixture
    {
        [Test]
        public async Task Machine_Save_DoesSave()
        {
            // Arrange
            Machine newMachine = new Machine
            {
                Name = Guid.NewGuid().ToString()
            };

            // Act
            await newMachine.Save(DbContext);

            Machine machine = DbContext.Machines.AsNoTracking().First();
            List<Machine> machineList = DbContext.Machines.AsNoTracking().ToList();
            List<MachineVersion> versionList = DbContext.MachineVersions.AsNoTracking().ToList();
            
            // Assert
            Assert.NotNull(machine);
            Assert.AreEqual(1, machineList.Count());
            Assert.AreEqual(1, versionList.Count());
            
            Assert.AreEqual(newMachine.Name, machine.Name);
        }

        [Test]
        public async Task Machine_Update_DoesUpdate()
        {
            // Arrange
            Machine machine = new Machine
            {
                Name = Guid.NewGuid().ToString()
            };

            DbContext.Machines.Add(machine);
            DbContext.SaveChanges();

            //Act
            Machine selectedMachine = new Machine
            {
                Name = Guid.NewGuid().ToString(),
                Id = machine.Id
            };

            await selectedMachine.Update(DbContext);

            Machine dbMachine = DbContext.Machines.AsNoTracking().First();
            List<Machine> machineList = DbContext.Machines.AsNoTracking().ToList();
            List<MachineVersion> versionList = DbContext.MachineVersions.AsNoTracking().ToList();

            //Assert
            
            Assert.NotNull(dbMachine);
            Assert.AreEqual(1, machineList.Count());
            Assert.AreEqual(1, versionList.Count());
            
            Assert.AreEqual(selectedMachine.Name, dbMachine.Name);
        }

        [Test]
        public async Task Machine_UpdateBinding_DoesUpdate()
        {
            // Arrange
            Machine machine = new Machine
            {
                Name = Guid.NewGuid().ToString()
            };

            Area area = new Area
            {
                Name = Guid.NewGuid().ToString()
            };

            DbContext.Machines.Add(machine);
            DbContext.Areas.Add(area);
            DbContext.SaveChanges();

            //Act
            Machine selectedMachine = new Machine
            {
                Name = Guid.NewGuid().ToString(),
                Id = machine.Id,
                MachineAreas = new List<Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea>()
                {
                    new Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea()
                    {
                        AreaId = area.Id,
                        MachineId = machine.Id
                    }
                }
            };

            await selectedMachine.Update(DbContext);

            //Assert
            Assert.AreEqual(selectedMachine.MachineAreas.First().AreaId, area.Id);

        }

        [Test]
        public async Task Machine_Delete_DoesDelete()
        {
            //Arrange
            Machine machine = new Machine
            {
                Name = Guid.NewGuid().ToString()
            };

            DbContext.Machines.Add(machine);
            DbContext.SaveChanges();

            //Act
            Machine selectedMachine = new Machine
            {
                Name = machine.Name,
                Id = machine.Id
            };

            await selectedMachine.Delete(DbContext);
            
            Machine dbMachine = DbContext.Machines.AsNoTracking().First();
            List<Machine> machineList = DbContext.Machines.AsNoTracking().ToList();
            List<MachineVersion> versionList = DbContext.MachineVersions.AsNoTracking().ToList();
            
            // Assert
            Assert.NotNull(dbMachine);
            Assert.AreEqual(1, machineList.Count());
            Assert.AreEqual(1, versionList.Count());
            
            Assert.AreEqual(dbMachine.WorkflowState, Constants.WorkflowStates.Removed);
        }
    }
}