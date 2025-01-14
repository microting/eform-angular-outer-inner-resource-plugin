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
        public async Task OuterResource_Save_DoesSave()
        {
           // Arrange
            OuterResource newOuterResource = new OuterResource()
            {
                Name = Guid.NewGuid().ToString()
            };
            // Act
            await newOuterResource.Create(DbContext);

            OuterResource area = DbContext.OuterResources.AsNoTracking().First();
            List<OuterResource> areaList = DbContext.OuterResources.AsNoTracking().ToList();
            List<OuterResourceVersion> versionList = DbContext.OuterResourceVersions.AsNoTracking().ToList();

            // Assert
            Assert.That(area, Is.Not.Null);
            Assert.That(areaList.Count, Is.EqualTo(1));
            Assert.That(versionList.Count, Is.EqualTo(1));

            Assert.That(area.Name, Is.EqualTo(newOuterResource.Name));
        }

        [Test]
        public async Task OuterResource_Update_DoesUpdate()
        {
            // Arrange
            OuterResource area = new OuterResource
            {
                Name = Guid.NewGuid().ToString()
            };

            await area.Create(DbContext);

            //Act

            var newName = Guid.NewGuid().ToString();

            area.Name = newName;
            await area.Update(DbContext);

            OuterResource dbOuterResource = DbContext.OuterResources.AsNoTracking().First();
            List<OuterResource> areaList = DbContext.OuterResources.AsNoTracking().ToList();
            List<OuterResourceVersion> versionList = DbContext.OuterResourceVersions.AsNoTracking().ToList();

            //Assert

            Assert.That(dbOuterResource, Is.Not.Null);
            Assert.That(areaList.Count, Is.EqualTo(1));
            Assert.That(versionList.Count, Is.EqualTo(2));

            Assert.That(dbOuterResource.Name, Is.EqualTo(newName));
        }

        [Test]
        public async Task OuterResource_UpdateBinding_DoesUpdate()
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
            OuterResource selectedOuterResource = new OuterResource
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

            await selectedOuterResource.Update(DbContext);

            //Assert
            Assert.That(machine.Id, Is.EqualTo(selectedOuterResource.OuterInnerResources.First().InnerResourceId));

        }

        [Test]
        public async Task OuterResource_Delete_DoesDelete()
        {
            //Arrange
            OuterResource area = new OuterResource
            {
                Name = Guid.NewGuid().ToString()
            };

            await area.Create(DbContext);

            //Act
            await area.Delete(DbContext);

            OuterResource dbOuterResource = DbContext.OuterResources.AsNoTracking().First();
            List<OuterResource> areaList = DbContext.OuterResources.AsNoTracking().ToList();
            List<OuterResourceVersion> versionList = DbContext.OuterResourceVersions.AsNoTracking().ToList();

            // Assert

            Assert.That(dbOuterResource, Is.Not.Null);
            Assert.That(areaList.Count, Is.EqualTo(1));
            Assert.That(versionList.Count, Is.EqualTo(2));

            Assert.That(Constants.WorkflowStates.Removed, Is.EqualTo(dbOuterResource.WorkflowState));
        }
    }
}