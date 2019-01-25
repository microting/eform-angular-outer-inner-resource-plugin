using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;
using NUnit.Framework;

namespace MachineArea.Pn.Test
{
    [TestFixture]
    public class MachinesUTest : DbTestFixture
    {
        [Test]
        public void MachineCreateTest()
        {
            // Arrange
            // Act
            List<Machine> machines = DbContext.Machines.AsNoTracking().ToList();
            
            // Assert
            Assert.NotNull(machines);
            Assert.AreEqual(0, machines.Count());
            
        }
    }
}