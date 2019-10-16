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
    public class OuterResourcesUTest : DbTestFixture
    {
        [Test]
        public async Task Area_Save_DoesSave()
        {
           // Arrange
            OuterResource newArea = new OuterResource()
            {
                Name = Guid.NewGuid().ToString()
            };
            // Act
            await newArea.Create(DbContext);

            OuterResource area = DbContext.OuterResources.AsNoTracking().First();
            List<OuterResource> areaList = DbContext.OuterResources.AsNoTracking().ToList();
            List<OuterResourceVersion> versionList = DbContext.OuterResourceVersions.AsNoTracking().ToList();
            
            // Assert
            Assert.NotNull(area);
            Assert.AreEqual(1, areaList.Count());
            Assert.AreEqual(1, versionList.Count());
            
            Assert.AreEqual(newArea.Name, area.Name);
        }

        [Test]
        public async Task Area_Update_DoesUpdate()
        {
            // Arrange
            OuterResource area = new OuterResource
            {
                Name = Guid.NewGuid().ToString()
            };

            DbContext.OuterResources.Add(area);
            DbContext.SaveChanges();

            //Act
            OuterResource selectedArea = new OuterResource
            {
                Name = Guid.NewGuid().ToString(),
                Id = area.Id
            };

            await selectedArea.Update(DbContext);

            OuterResource dbArea = DbContext.OuterResources.AsNoTracking().First();
            List<OuterResource> areaList = DbContext.OuterResources.AsNoTracking().ToList();
            List<OuterResourceVersion> versionList = DbContext.OuterResourceVersions.AsNoTracking().ToList();

            //Assert
            
            Assert.NotNull(dbArea);
            Assert.AreEqual(1, areaList.Count());
            Assert.AreEqual(1, versionList.Count());
            
            Assert.AreEqual(selectedArea.Name, dbArea.Name);
        }

        [Test]
        public async Task Area_UpdateBinding_DoesUpdate()
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
            OuterResource selectedArea = new OuterResource
            {
                Name = Guid.NewGuid().ToString(),
                Id = area.Id,
                OuterInnerResources = new List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource>()
                {
                    new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource()
                    {
                        OuterResourceId = area.Id,
                        InnerResourceId = machine.Id
                    }
                }
            };

            await selectedArea.Update(DbContext);

            //Assert
            Assert.AreEqual(selectedArea.OuterInnerResources.First().InnerResourceId, machine.Id);

        }

        [Test]
        public async Task Area_Delete_DoesDelete()
        {
            //Arrange
            OuterResource area = new OuterResource
            {
                Name = Guid.NewGuid().ToString()
            };

            DbContext.OuterResources.Add(area);
            DbContext.SaveChanges();

            //Act
            OuterResource selectedArea = new OuterResource
            {
                Id = area.Id
            };

            await selectedArea.Delete(DbContext);
            
            OuterResource dbArea = DbContext.OuterResources.AsNoTracking().First();
            List<OuterResource> areaList = DbContext.OuterResources.AsNoTracking().ToList();
            List<OuterResourceVersion> versionList = DbContext.OuterResourceVersions.AsNoTracking().ToList();
            
            // Assert
            
            Assert.NotNull(dbArea);
            Assert.AreEqual(1, areaList.Count());
            Assert.AreEqual(1, versionList.Count());
            
            Assert.AreEqual(dbArea.WorkflowState, Constants.WorkflowStates.Removed);
        }
    }
}