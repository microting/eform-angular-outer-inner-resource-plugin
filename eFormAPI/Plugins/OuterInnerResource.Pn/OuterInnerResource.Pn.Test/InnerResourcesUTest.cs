/*
The MIT License (MIT)

Copyright (c) 2007 - 2019 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Infrastructure.Constants;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using NUnit.Framework;

namespace MachineArea.Pn.Test
{
    [TestFixture]
    public class InnerResourcesUTest : DbTestFixture
    {
        [Test]
        public async Task Machine_Save_DoesSave()
        {
            // Arrange
            InnerResource newInnerResource = new InnerResource
            {
                Name = Guid.NewGuid().ToString()
            };

            // Act
            await newInnerResource.Create(DbContext);

            InnerResource machine = DbContext.InnerResources.AsNoTracking().First();
            List<InnerResource> machineList = DbContext.InnerResources.AsNoTracking().ToList();
            List<InnerResourceVersion> versionList = DbContext.InnerResourceVersions.AsNoTracking().ToList();
            
            // Assert
            Assert.NotNull(machine);
            Assert.AreEqual(1, machineList.Count());
            Assert.AreEqual(1, versionList.Count());
            
            Assert.AreEqual(newInnerResource.Name, machine.Name);
        }

        [Test]
        public async Task Machine_Update_DoesUpdate()
        {
            // Arrange
            InnerResource machine = new InnerResource
            {
                Name = Guid.NewGuid().ToString()
            };

            DbContext.InnerResources.Add(machine);
            DbContext.SaveChanges();

            //Act
            InnerResource selectedInnerResource = new InnerResource
            {
                Name = Guid.NewGuid().ToString(),
                Id = machine.Id
            };

            await selectedInnerResource.Update(DbContext);

            InnerResource dbInnerResource = DbContext.InnerResources.AsNoTracking().First();
            List<InnerResource> machineList = DbContext.InnerResources.AsNoTracking().ToList();
            List<InnerResourceVersion> versionList = DbContext.InnerResourceVersions.AsNoTracking().ToList();

            //Assert
            
            Assert.NotNull(dbInnerResource);
            Assert.AreEqual(1, machineList.Count());
            Assert.AreEqual(1, versionList.Count());
            
            Assert.AreEqual(selectedInnerResource.Name, dbInnerResource.Name);
        }

        [Test]
        public async Task Machine_UpdateBinding_DoesUpdate()
        {
            // Arrange
            InnerResource machine = new InnerResource
            {
                Name = Guid.NewGuid().ToString()
            };

            OuterResource area = new OuterResource
            {
                Name = Guid.NewGuid().ToString()
            };

            DbContext.InnerResources.Add(machine);
            DbContext.OuterResources.Add(area);
            DbContext.SaveChanges();

            //Act
            InnerResource selectedInnerResource = new InnerResource
            {
                Name = Guid.NewGuid().ToString(),
                Id = machine.Id,
                OuterInnerResources = new List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource>()
                {
                    new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource()
                    {
                        OuterResourceId = area.Id,
                        InnerResourceId = machine.Id
                    }
                }
            };

            await selectedInnerResource.Update(DbContext);

            //Assert
            Assert.AreEqual(selectedInnerResource.OuterInnerResources.First().OuterResourceId, area.Id);

        }

        [Test]
        public async Task Machine_Delete_DoesDelete()
        {
            //Arrange
            InnerResource machine = new InnerResource
            {
                Name = Guid.NewGuid().ToString()
            };

            DbContext.InnerResources.Add(machine);
            DbContext.SaveChanges();

            //Act
            InnerResource selectedInnerResource = new InnerResource
            {
                Name = machine.Name,
                Id = machine.Id
            };

            await selectedInnerResource.Delete(DbContext);
            
            InnerResource dbInnerResource = DbContext.InnerResources.AsNoTracking().First();
            List<InnerResource> machineList = DbContext.InnerResources.AsNoTracking().ToList();
            List<InnerResourceVersion> versionList = DbContext.InnerResourceVersions.AsNoTracking().ToList();
            
            // Assert
            Assert.NotNull(dbInnerResource);
            Assert.AreEqual(1, machineList.Count());
            Assert.AreEqual(1, versionList.Count());
            
            Assert.AreEqual(dbInnerResource.WorkflowState, Constants.WorkflowStates.Removed);
        }
    }
}