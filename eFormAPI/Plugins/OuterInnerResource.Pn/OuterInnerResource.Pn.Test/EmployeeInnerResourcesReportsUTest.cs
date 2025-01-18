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
using Microting.eForm.Dto;
using Microting.eForm.Infrastructure.Data.Entities;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using NUnit.Framework;
using OuterInnerResource.Pn.Infrastructure.Enums;
using OuterInnerResource.Pn.Infrastructure.Helpers;
using OuterInnerResource.Pn.Infrastructure.Models.Report;

namespace MachineArea.Pn.Test
{
    [TestFixture]
    public class EmployeeInnerResourcesReportsUTest : DbTestFixture
    {
        [Test]
        public async Task EmployeeMachinesByDay_Generate_DoesGenerate()
        {
            OuterResource newArea = new OuterResource() { Name = "My OuterResource 1", Version = 1 };
            OuterResource newArea1 = new OuterResource() { Name = "My OuterResource 2", Version = 1 };
            OuterResource newArea2 = new OuterResource() { Name = "My OuterResource 3", Version = 1 };
            InnerResource newMachine = new InnerResource() { Name = "My InnerResource 1", Version = 1 };
            InnerResource newMachine1 = new InnerResource() { Name = "My InnerResource 2", Version = 1 };
            InnerResource newMachine2 = new InnerResource() { Name = "My InnerResource 3", Version = 1 };
            await newArea.Create(DbContext);
            await newArea1.Create(DbContext);
            await newArea2.Create(DbContext);
            await newMachine.Create(DbContext);
            await newMachine1.Create(DbContext);
            await newMachine2.Create(DbContext);

            // Different days
            ResourceTimeRegistration newTimeRegistrationDay = new ResourceTimeRegistration()
            { OuterResourceId = newArea.Id, Version = 1, SDKSiteId = 1, InnerResourceId = newMachine.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 13) };
            ResourceTimeRegistration newTimeRegistrationDay1 = new ResourceTimeRegistration()
            { OuterResourceId = newArea1.Id, Version = 1, SDKSiteId = 1, InnerResourceId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 14) };
            ResourceTimeRegistration newTimeRegistrationDay2 = new ResourceTimeRegistration()
            { OuterResourceId = newArea2.Id, Version = 1, SDKSiteId = 1, InnerResourceId = newMachine2.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 15) };
            ResourceTimeRegistration newTimeRegistrationDay3 = new ResourceTimeRegistration()
            { OuterResourceId = newArea.Id, Version = 1, SDKSiteId = 2, InnerResourceId = newMachine.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 13) };
            ResourceTimeRegistration newTimeRegistrationDay4 = new ResourceTimeRegistration()
            { OuterResourceId = newArea1.Id, Version = 1, SDKSiteId = 2, InnerResourceId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 14) };
            ResourceTimeRegistration newTimeRegistrationDay5 = new ResourceTimeRegistration()
            { OuterResourceId = newArea1.Id, Version = 1, SDKSiteId = 2, InnerResourceId = newMachine2.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 15) };
            ResourceTimeRegistration newTimeRegistrationDay6 = new ResourceTimeRegistration()
            { OuterResourceId = newArea1.Id, Version = 1, SDKSiteId = 2, InnerResourceId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 15) };


            await DbContext.ResourceTimeRegistrations.AddRangeAsync(newTimeRegistrationDay, newTimeRegistrationDay1,
                newTimeRegistrationDay2, newTimeRegistrationDay3, newTimeRegistrationDay4, newTimeRegistrationDay5,
                newTimeRegistrationDay6);

            await DbContext.SaveChangesAsync();

            GenerateReportModel model = new GenerateReportModel()
            {
                DateFrom = new DateTime(2019, 5, 13),
                DateTo = new DateTime(2019, 5, 16),
                Relationship = ReportRelationshipType.EmployeeInnerResource,
                Type = ReportType.Day
            };

            List<Site> sitesList = new List<Site>()
            {
                new Site()
                {
                    Name = "Test Site 1",
                    MicrotingUid = 1
                },
                new Site()
                {
                    Name = "Test Site 2",
                    MicrotingUid = 2
                }
            };

            DateTime modelDateFrom = new DateTime(
                model.DateFrom.Year,
                model.DateFrom.Month,
                model.DateFrom.Day,
                0, 0, 0);
            DateTime modelDateTo = new DateTime(
                model.DateTo.Year,
                model.DateTo.Month,
                model.DateTo.Day,
                23, 59, 59);

            List<ResourceTimeRegistration> jobsList = await DbContext.ResourceTimeRegistrations
                .Include(x => x.InnerResource)
                .Include(x => x.OuterResource)
                .Where(x => x.DoneAt >= modelDateFrom && x.DoneAt <= modelDateTo)
                .ToListAsync();

            ReportModel reportModel = ReportsHelper.GetReportData(model, jobsList, sitesList, (int)ReportTimeType.Minutes);

            Assert.That(2, Is.EqualTo(reportModel.SubReports.Count));

            Assert.That(1800, Is.EqualTo(reportModel.SubReports[0].TotalTime));
            Assert.That(3, Is.EqualTo(reportModel.SubReports[0].Entities.Count));
            Assert.That(sitesList[0].Name, Is.EqualTo(reportModel.SubReports[0].Entities[0].EntityName));
            Assert.That(newMachine.Name, Is.EqualTo(reportModel.SubReports[0].Entities[0].RelatedEntityName));
            Assert.That(sitesList[0].Name, Is.EqualTo(reportModel.SubReports[0].Entities[1].EntityName));
            Assert.That(newMachine1.Name, Is.EqualTo(reportModel.SubReports[0].Entities[1].RelatedEntityName));
            Assert.That(sitesList[0].Name, Is.EqualTo(reportModel.SubReports[0].Entities[2].EntityName));
            Assert.That(newMachine2.Name, Is.EqualTo(reportModel.SubReports[0].Entities[2].RelatedEntityName));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[0].Entities[0].TotalTime));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[0].Entities[1].TotalTime));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[0]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[1]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[2]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[3]));

            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[0]));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[1]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[2]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[3]));

            Assert.That(2400, Is.EqualTo(reportModel.SubReports[1].TotalTime));
            Assert.That(sitesList[1].Name, Is.EqualTo(reportModel.SubReports[1].Entities[0].EntityName));
            Assert.That(newMachine.Name, Is.EqualTo(reportModel.SubReports[1].Entities[0].RelatedEntityName));
            Assert.That(sitesList[1].Name, Is.EqualTo(reportModel.SubReports[1].Entities[1].EntityName));
            Assert.That(newMachine1.Name, Is.EqualTo(reportModel.SubReports[1].Entities[1].RelatedEntityName));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[1].Entities[0].TotalTime));
            Assert.That(1200, Is.EqualTo(reportModel.SubReports[1].Entities[1].TotalTime));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[0]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[1]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[2]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[3]));

            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[1].TimePerTimeUnit[0]));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[1].Entities[1].TimePerTimeUnit[1]));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[1].Entities[1].TimePerTimeUnit[2]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[1].TimePerTimeUnit[3]));
        }

        [Test]
        public async Task EmployeeMachinesReportByWeek_Generate_DoesGenerate()
        {
            OuterResource newArea = new OuterResource() { Name = "My OuterResource 1", Version = 1 };
            OuterResource newArea1 = new OuterResource() { Name = "My OuterResource 2", Version = 1 };
            OuterResource newArea2 = new OuterResource() { Name = "My OuterResource 3", Version = 1 };
            InnerResource newMachine = new InnerResource() { Name = "My InnerResource 1", Version = 1 };
            InnerResource newMachine1 = new InnerResource() { Name = "My InnerResource 2", Version = 1 };
            InnerResource newMachine2 = new InnerResource() { Name = "My InnerResource 3", Version = 1 };

            await newArea.Create(DbContext);
            await newArea1.Create(DbContext);
            await newArea2.Create(DbContext);
            await newMachine.Create(DbContext);
            await newMachine1.Create(DbContext);
            await newMachine2.Create(DbContext);


            // Different Weeks
            ResourceTimeRegistration newTimeRegistrationWeek = new ResourceTimeRegistration()
            { OuterResourceId = newArea.Id, Version = 1, SDKSiteId = 1, InnerResourceId = newMachine.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 20) };
            ResourceTimeRegistration newTimeRegistrationWeek1 = new ResourceTimeRegistration()
            { OuterResourceId = newArea1.Id, Version = 1, SDKSiteId = 1, InnerResourceId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 20) };
            ResourceTimeRegistration newTimeRegistrationWeek2 = new ResourceTimeRegistration()
            { OuterResourceId = newArea2.Id, Version = 1, SDKSiteId = 1, InnerResourceId = newMachine2.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 27) };
            ResourceTimeRegistration newTimeRegistrationWeek3 = new ResourceTimeRegistration()
            { OuterResourceId = newArea.Id, Version = 1, SDKSiteId = 2, InnerResourceId = newMachine.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 06, 27) };
            ResourceTimeRegistration newTimeRegistrationWeek4 = new ResourceTimeRegistration()
            { OuterResourceId = newArea1.Id, Version = 1, SDKSiteId = 2, InnerResourceId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 06, 05) };
            ResourceTimeRegistration newTimeRegistrationWeek5 = new ResourceTimeRegistration()
            { OuterResourceId = newArea1.Id, Version = 1, SDKSiteId = 2, InnerResourceId = newMachine2.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 06, 05) };
            ResourceTimeRegistration newTimeRegistrationWeek6 = new ResourceTimeRegistration()
            { OuterResourceId = newArea1.Id, Version = 1, SDKSiteId = 2, InnerResourceId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 06, 06) };

            await DbContext.ResourceTimeRegistrations.AddRangeAsync(newTimeRegistrationWeek, newTimeRegistrationWeek1,
                newTimeRegistrationWeek2, newTimeRegistrationWeek3, newTimeRegistrationWeek4, newTimeRegistrationWeek5,
                newTimeRegistrationWeek6);

            await DbContext.SaveChangesAsync();

            GenerateReportModel model = new GenerateReportModel()
            {
                DateFrom = new DateTime(2019, 5, 13),
                DateTo = new DateTime(2019, 6, 07),
                Relationship = ReportRelationshipType.EmployeeInnerResource,
                Type = ReportType.Week
            };

            List<Site> sitesList = new List<Site>()
            {
                new Site()
                {
                    Name = "Test Site 1",
                    MicrotingUid = 1
                },
                new Site()
                {
                    Name = "Test Site 2",
                    MicrotingUid = 2
                }
            };

            DateTime modelDateFrom = new DateTime(
                model.DateFrom.Year,
                model.DateFrom.Month,
                model.DateFrom.Day,
                0, 0, 0);
            DateTime modelDateTo = new DateTime(
                model.DateTo.Year,
                model.DateTo.Month,
                model.DateTo.Day,
                23, 59, 59);

            List<ResourceTimeRegistration> jobsList = await DbContext.ResourceTimeRegistrations
                .Include(x => x.InnerResource)
                .Include(x => x.OuterResource)
                .Where(x => x.DoneAt >= modelDateFrom && x.DoneAt <= modelDateTo)
                .ToListAsync();

            ReportModel reportModel = ReportsHelper.GetReportData(model, jobsList, sitesList, (int)ReportTimeType.Minutes);

            Assert.That(2, Is.EqualTo(reportModel.SubReports.Count));

            Assert.That(1800, Is.EqualTo(reportModel.SubReports[0].TotalTime));
            Assert.That(3, Is.EqualTo(reportModel.SubReports[0].Entities.Count));
            Assert.That(sitesList[0].Name, Is.EqualTo(reportModel.SubReports[0].Entities[0].EntityName));
            Assert.That(newMachine.Name, Is.EqualTo(reportModel.SubReports[0].Entities[0].RelatedEntityName));
            Assert.That(sitesList[0].Name, Is.EqualTo(reportModel.SubReports[0].Entities[1].EntityName));
            Assert.That(newMachine1.Name, Is.EqualTo(reportModel.SubReports[0].Entities[1].RelatedEntityName));
            Assert.That(sitesList[0].Name, Is.EqualTo(reportModel.SubReports[0].Entities[2].EntityName));
            Assert.That(newMachine2.Name, Is.EqualTo(reportModel.SubReports[0].Entities[2].RelatedEntityName));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[0].Entities[0].TotalTime));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[0].Entities[1].TotalTime));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[0]));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[1]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[2]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[3]));

            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[0]));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[1]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[2]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[3]));

            Assert.That(1800, Is.EqualTo(reportModel.SubReports[1].TotalTime));
            Assert.That(sitesList[1].Name, Is.EqualTo(reportModel.SubReports[1].Entities[0].EntityName));
            Assert.That(newMachine1.Name, Is.EqualTo(reportModel.SubReports[1].Entities[0].RelatedEntityName));
            Assert.That(1200, Is.EqualTo(reportModel.SubReports[1].Entities[0].TotalTime));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[0]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[1]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[2]));
            Assert.That(1200, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[3]));
        }

        [Test]
        public async Task EmployeeMachinesReportByMonth_Generate_DoesGenerate()
        {
            OuterResource newArea = new OuterResource() { Name = "My OuterResource 1", Version = 1 };
            OuterResource newArea1 = new OuterResource() { Name = "My OuterResource 2", Version = 1 };
            OuterResource newArea2 = new OuterResource() { Name = "My OuterResource 3", Version = 1 };
            InnerResource newMachine = new InnerResource() { Name = "My InnerResource 1", Version = 1 };
            InnerResource newMachine1 = new InnerResource() { Name = "My InnerResource 2", Version = 1 };
            InnerResource newMachine2 = new InnerResource() { Name = "My InnerResource 3", Version = 1 };

            await newArea.Create(DbContext);
            await newArea1.Create(DbContext);
            await newArea2.Create(DbContext);
            await newMachine.Create(DbContext);
            await newMachine1.Create(DbContext);
            await newMachine2.Create(DbContext);

            // Different Month
            ResourceTimeRegistration newTimeRegistrationMonth = new ResourceTimeRegistration()
            { OuterResourceId = newArea.Id, Version = 1, SDKSiteId = 1, InnerResourceId = newMachine.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 06, 13) };
            ResourceTimeRegistration newTimeRegistrationMonth1 = new ResourceTimeRegistration()
            { OuterResourceId = newArea1.Id, Version = 1, SDKSiteId = 1, InnerResourceId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 07, 14) };
            ResourceTimeRegistration newTimeRegistrationMonth2 = new ResourceTimeRegistration()
            { OuterResourceId = newArea2.Id, Version = 1, SDKSiteId = 1, InnerResourceId = newMachine2.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 07, 15) };
            ResourceTimeRegistration newTimeRegistrationMonth3 = new ResourceTimeRegistration()
            { OuterResourceId = newArea.Id, Version = 1, SDKSiteId = 2, InnerResourceId = newMachine.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 08, 13) };
            ResourceTimeRegistration newTimeRegistrationMonth4 = new ResourceTimeRegistration()
            { OuterResourceId = newArea1.Id, Version = 1, SDKSiteId = 2, InnerResourceId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 08, 14) };
            ResourceTimeRegistration newTimeRegistrationMonth5 = new ResourceTimeRegistration()
            { OuterResourceId = newArea1.Id, Version = 1, SDKSiteId = 2, InnerResourceId = newMachine2.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 09, 15) };
            ResourceTimeRegistration newTimeRegistrationMonth6 = new ResourceTimeRegistration()
            { OuterResourceId = newArea1.Id, Version = 1, SDKSiteId = 2, InnerResourceId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 09, 15) };

            await DbContext.ResourceTimeRegistrations.AddRangeAsync(newTimeRegistrationMonth, newTimeRegistrationMonth1,
                newTimeRegistrationMonth2, newTimeRegistrationMonth3, newTimeRegistrationMonth4, newTimeRegistrationMonth5,
                newTimeRegistrationMonth6);

            await DbContext.SaveChangesAsync();

            GenerateReportModel model = new GenerateReportModel()
            {
                DateFrom = new DateTime(2019, 5, 13),
                DateTo = new DateTime(2019, 9, 13),
                Relationship = ReportRelationshipType.EmployeeInnerResource,
                Type = ReportType.Month
            };

            List<Site> sitesList = new List<Site>()
            {
                new Site()
                {
                    Name = "Test Site 1",
                    MicrotingUid = 1
                },
                new Site()
                {
                    Name = "Test Site 2",
                    MicrotingUid = 2
                }
            };

            DateTime modelDateFrom = new DateTime(
                model.DateFrom.Year,
                model.DateFrom.Month,
                model.DateFrom.Day,
                0, 0, 0);
            DateTime modelDateTo = new DateTime(
                model.DateTo.Year,
                model.DateTo.Month,
                model.DateTo.Day,
                23, 59, 59);

            List<ResourceTimeRegistration> jobsList = await DbContext.ResourceTimeRegistrations
                .Include(x => x.InnerResource)
                .Include(x => x.OuterResource)
                .Where(x => x.DoneAt >= modelDateFrom && x.DoneAt <= modelDateTo)
                .ToListAsync();

            ReportModel reportModel = ReportsHelper.GetReportData(model, jobsList, sitesList, (int)ReportTimeType.Minutes);

            Assert.That(2, Is.EqualTo(reportModel.SubReports.Count));

            Assert.That(1800, Is.EqualTo(reportModel.SubReports[0].TotalTime));
            Assert.That(3, Is.EqualTo(reportModel.SubReports[0].Entities.Count));
            Assert.That(sitesList[0].Name, Is.EqualTo(reportModel.SubReports[0].Entities[0].EntityName));
            Assert.That(newMachine.Name, Is.EqualTo(reportModel.SubReports[0].Entities[0].RelatedEntityName));
            Assert.That(sitesList[0].Name, Is.EqualTo(reportModel.SubReports[0].Entities[1].EntityName));
            Assert.That(newMachine1.Name, Is.EqualTo(reportModel.SubReports[0].Entities[1].RelatedEntityName));
            Assert.That(sitesList[0].Name, Is.EqualTo(reportModel.SubReports[0].Entities[2].EntityName));
            Assert.That(newMachine2.Name, Is.EqualTo(reportModel.SubReports[0].Entities[2].RelatedEntityName));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[0].Entities[0].TotalTime));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[0].Entities[1].TotalTime));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[0]));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[1]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[2]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[3]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[4]));

            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[0]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[1]));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[2]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[3]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[4]));

            Assert.That(1200, Is.EqualTo(reportModel.SubReports[1].TotalTime));
            Assert.That(sitesList[1].Name, Is.EqualTo(reportModel.SubReports[1].Entities[0].EntityName));
            Assert.That(newMachine.Name, Is.EqualTo(reportModel.SubReports[1].Entities[0].RelatedEntityName));
            Assert.That(sitesList[1].Name, Is.EqualTo(reportModel.SubReports[1].Entities[1].EntityName));
            Assert.That(newMachine1.Name, Is.EqualTo(reportModel.SubReports[1].Entities[1].RelatedEntityName));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[1].Entities[0].TotalTime));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[1].Entities[1].TotalTime));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[0]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[1]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[2]));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[3]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[0].TimePerTimeUnit[4]));

            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[1].TimePerTimeUnit[0]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[1].TimePerTimeUnit[1]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[1].TimePerTimeUnit[2]));
            Assert.That(600, Is.EqualTo(reportModel.SubReports[1].Entities[1].TimePerTimeUnit[3]));
            Assert.That(0, Is.EqualTo(reportModel.SubReports[1].Entities[1].TimePerTimeUnit[4]));
        }
    }
}