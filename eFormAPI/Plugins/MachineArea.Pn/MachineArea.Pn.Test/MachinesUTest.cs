using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using eFormShared;
using MachineArea.Pn.Infrastructure.Models.Machines;
using Microsoft.EntityFrameworkCore;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;
using NUnit.Framework;

namespace MachineArea.Pn.Test
{
    [TestFixture]
    public class MachinesUTest : DbTestFixture
    {
        [Test]
        public async Task MachineModel_Save_DoesSave()
        {
            // Arrange
            MachineModel machineModel = new MachineModel();
            machineModel.Name = Guid.NewGuid().ToString();
            // Act
            await machineModel.Save(DbContext);

            Machine machine = DbContext.Machines.AsNoTracking().First();
            List<Machine> machineList = DbContext.Machines.AsNoTracking().ToList();
            List<MachineVersion> versionList = DbContext.MachineVersions.AsNoTracking().ToList();
            
            // Assert
            Assert.NotNull(machine);
            Assert.AreEqual(1, machineList.Count());
            Assert.AreEqual(1, versionList.Count());
            
            Assert.AreEqual(machineModel.Name, machine.Name);
        }

        [Test]
        public async Task MachineModel_Update_DoesUpdate()
        {
            // Arrange
            Machine machine = new Machine();
            machine.Name = Guid.NewGuid().ToString();

            DbContext.Machines.Add(machine);
            DbContext.SaveChanges();
            
            //Act
            MachineModel machineModel = new MachineModel();
            machineModel.Name = Guid.NewGuid().ToString();
            machineModel.Id = machine.Id;

            await machineModel.Update(DbContext);

            Machine dbMachine = DbContext.Machines.AsNoTracking().First();
            List<Machine> machineList = DbContext.Machines.AsNoTracking().ToList();
            List<MachineVersion> versionList = DbContext.MachineVersions.AsNoTracking().ToList();

            //Assert
            
            Assert.NotNull(dbMachine);
            Assert.AreEqual(1, machineList);
            Assert.AreEqual(1, versionList);
            
            Assert.AreEqual(machineModel.Name, dbMachine.Name);
        }

        [Test]
        public async Task MachineModel_Delete_DoesDelete()
        {
            //Arrange
            Machine machine = new Machine();
            machine.Name = Guid.NewGuid().ToString();

            DbContext.Machines.Add(machine);
            DbContext.SaveChanges();
            
            //Act
            MachineModel machineModel = new MachineModel();
            machineModel.Name = machine.Name;
            machineModel.Id = machine.Id;

            await machineModel.Delete(DbContext);
            
            Machine dbMachine = DbContext.Machines.AsNoTracking().First();
            List<Machine> machineList = DbContext.Machines.AsNoTracking().ToList();
            List<MachineVersion> versionList = DbContext.MachineVersions.AsNoTracking().ToList();
            
            // Assert
            
            Assert.NotNull(dbMachine);
            Assert.AreEqual(1, machineList);
            Assert.AreEqual(1, versionList);
            
            Assert.AreEqual(dbMachine.Name, machineModel.Name);
            Assert.AreEqual(dbMachine.WorkflowState, Constants.WorkflowStates.Removed);
        }
    }
}