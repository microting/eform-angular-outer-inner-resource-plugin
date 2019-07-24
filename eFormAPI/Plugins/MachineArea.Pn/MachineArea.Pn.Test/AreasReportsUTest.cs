using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MachineArea.Pn.Infrastructure.Enums;
using MachineArea.Pn.Infrastructure.Helpers;
using MachineArea.Pn.Infrastructure.Models.Report;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Dto;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;
using NUnit.Framework;


namespace MachineArea.Pn.Test
{
    [TestFixture]
    public class AreasReportUTest : DbTestFixture
    {
        [Test]
        public async Task AreasReportByDay_Generate_DoesGenerate()
        {
            Area newArea = new Area() { Name = "My Area 1", Version = 1 };
            Area newArea1 = new Area() { Name = "My Area 2", Version = 1 };
            Area newArea2 = new Area() { Name = "My Area 3", Version = 1 };
            Machine newMachine = new Machine() { Name = "My Machine 1", Version = 1 };
            Machine newMachine1 = new Machine() { Name = "My Machine 2", Version = 1 };
            Machine newMachine2 = new Machine() { Name = "My Machine 3", Version = 1 };
            await newArea.Create(DbContext);
            await newArea1.Create(DbContext);
            await newArea2.Create(DbContext);
            await newMachine.Create(DbContext);
            await newMachine1.Create(DbContext);
            await newMachine2.Create(DbContext);

            // Different days
            MachineAreaTimeRegistration newTimeRegistrationDay = new MachineAreaTimeRegistration()
            { AreaId = newArea.Id, Version = 1, SDKSiteId = 1, MachineId = newMachine.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 13) };
            MachineAreaTimeRegistration newTimeRegistrationDay1 = new MachineAreaTimeRegistration()
            { AreaId = newArea1.Id, Version = 1, SDKSiteId = 1, MachineId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 14) };
            MachineAreaTimeRegistration newTimeRegistrationDay2 = new MachineAreaTimeRegistration()
            { AreaId = newArea2.Id, Version = 1, SDKSiteId = 1, MachineId = newMachine2.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 15) };
            MachineAreaTimeRegistration newTimeRegistrationDay3 = new MachineAreaTimeRegistration()
            { AreaId = newArea.Id, Version = 1, SDKSiteId = 2, MachineId = newMachine.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 13) };
            MachineAreaTimeRegistration newTimeRegistrationDay4 = new MachineAreaTimeRegistration()
            { AreaId = newArea1.Id, Version = 1, SDKSiteId = 2, MachineId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 14) };
            MachineAreaTimeRegistration newTimeRegistrationDay5 = new MachineAreaTimeRegistration()
            { AreaId = newArea1.Id, Version = 1, SDKSiteId = 2, MachineId = newMachine2.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 15) };
            MachineAreaTimeRegistration newTimeRegistrationDay6 = new MachineAreaTimeRegistration()
            { AreaId = newArea1.Id, Version = 1, SDKSiteId = 2, MachineId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 15) };


            await DbContext.MachineAreaTimeRegistrations.AddRangeAsync(newTimeRegistrationDay, newTimeRegistrationDay1,
                newTimeRegistrationDay2, newTimeRegistrationDay3, newTimeRegistrationDay4, newTimeRegistrationDay5,
                newTimeRegistrationDay6);

            await DbContext.SaveChangesAsync();

            GenerateReportModel model = new GenerateReportModel()
            {
                DateFrom = new DateTime(2019, 5, 13),
                DateTo = new DateTime(2019, 5, 16),
                Relationship = ReportRelationshipType.Area,
                Type = ReportType.Day
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

            List<Site_Dto> sitesList = new List<Site_Dto>()
            {
                new Site_Dto(1, "Test Site 1", "", "", 1, 1, 1, 1),
                new Site_Dto(2, "Test Site 2", "", "", 1, 1, 1, 1)
            };

            List<MachineAreaTimeRegistration> jobsList = await DbContext.MachineAreaTimeRegistrations
                .Include(x => x.Machine)
                .Include(x => x.Area)
                .Where(x => x.DoneAt >= modelDateFrom && x.DoneAt <= modelDateTo)
                .ToListAsync();

            ReportModel reportModel = ReportsHelper.GetReportData(model, jobsList, sitesList, (int)ReportTimeType.Minutes);

            Assert.AreEqual(reportModel.SubReports.Count, 1);
            Assert.AreEqual(reportModel.SubReports[0].TotalTime, 4200);
            Assert.AreEqual(reportModel.SubReports[0].Entities.Count, 3);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].EntityName, newArea.Name);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].EntityName, newArea1.Name);
            Assert.AreEqual(reportModel.SubReports[0].Entities[2].EntityName, newArea2.Name);

            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TotalTime, 1200);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TotalTime, 2400);

            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[0], 1200);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[1], 0);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[2], 0);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[3], 0);

            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[0], 0);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[1], 1200);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[2], 1200);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[3], 0);
        }

        [Test]
        public async Task AreasReportByWeek_Generate_DoesGenerate()
        {
            Area newArea = new Area() { Name = "My Area 1", Version = 1 };
            Area newArea1 = new Area() { Name = "My Area 2", Version = 1 };
            Area newArea2 = new Area() { Name = "My Area 3", Version = 1 };
            Machine newMachine = new Machine() { Name = "My Machine 1", Version = 1 };
            Machine newMachine1 = new Machine() { Name = "My Machine 2", Version = 1 };
            Machine newMachine2 = new Machine() { Name = "My Machine 3", Version = 1 };

            await newArea.Create(DbContext);
            await newArea1.Create(DbContext);
            await newArea2.Create(DbContext);
            await newMachine.Create(DbContext);
            await newMachine1.Create(DbContext);
            await newMachine2.Create(DbContext);

            // Different Weeks
            MachineAreaTimeRegistration newTimeRegistrationWeek = new MachineAreaTimeRegistration()
            { AreaId = newArea.Id, Version = 1, SDKSiteId = 1, MachineId = newMachine.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 20) };
            MachineAreaTimeRegistration newTimeRegistrationWeek1 = new MachineAreaTimeRegistration()
            { AreaId = newArea1.Id, Version = 1, SDKSiteId = 1, MachineId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 20) };
            MachineAreaTimeRegistration newTimeRegistrationWeek2 = new MachineAreaTimeRegistration()
            { AreaId = newArea2.Id, Version = 1, SDKSiteId = 1, MachineId = newMachine2.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 05, 27) };
            MachineAreaTimeRegistration newTimeRegistrationWeek3 = new MachineAreaTimeRegistration()
            { AreaId = newArea.Id, Version = 1, SDKSiteId = 2, MachineId = newMachine.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 06, 27) };
            MachineAreaTimeRegistration newTimeRegistrationWeek4 = new MachineAreaTimeRegistration()
            { AreaId = newArea1.Id, Version = 1, SDKSiteId = 2, MachineId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 06, 05) };
            MachineAreaTimeRegistration newTimeRegistrationWeek5 = new MachineAreaTimeRegistration()
            { AreaId = newArea1.Id, Version = 1, SDKSiteId = 2, MachineId = newMachine2.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 06, 05) };
            MachineAreaTimeRegistration newTimeRegistrationWeek6 = new MachineAreaTimeRegistration()
            { AreaId = newArea1.Id, Version = 1, SDKSiteId = 2, MachineId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 06, 06) };

            await DbContext.MachineAreaTimeRegistrations.AddRangeAsync(newTimeRegistrationWeek, newTimeRegistrationWeek1,
                newTimeRegistrationWeek2, newTimeRegistrationWeek3, newTimeRegistrationWeek4, newTimeRegistrationWeek5,
                newTimeRegistrationWeek6);

            await DbContext.SaveChangesAsync();

            GenerateReportModel model = new GenerateReportModel()
            {
                DateFrom = new DateTime(2019, 5, 13),
                DateTo = new DateTime(2019, 6, 07),
                Relationship = ReportRelationshipType.Area,
                Type = ReportType.Week
            };

            List<Site_Dto> sitesList = new List<Site_Dto>()
            {
                new Site_Dto(1, "Test Site 1", "", "", 1, 1, 1, 1),
                new Site_Dto(2, "Test Site 2", "", "", 1, 1, 1, 1)
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

            List<MachineAreaTimeRegistration> jobsList = await DbContext.MachineAreaTimeRegistrations
                .Include(x => x.Machine)
                .Include(x => x.Area)
                .Where(x => x.DoneAt >= modelDateFrom && x.DoneAt <= modelDateTo)
                .ToListAsync();

            ReportModel reportModel = ReportsHelper.GetReportData(model, jobsList, sitesList, (int)ReportTimeType.Minutes);

            Assert.AreEqual(reportModel.SubReports.Count, 1);
            Assert.AreEqual(reportModel.SubReports[0].TotalTime, 3600);
            Assert.AreEqual(reportModel.SubReports[0].Entities.Count, 3);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].EntityName, newArea.Name);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].EntityName, newArea1.Name);
            Assert.AreEqual(reportModel.SubReports[0].Entities[2].EntityName, newArea2.Name);

            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TotalTime, 600);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TotalTime, 2400);

            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[0], 0);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[1], 600);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[2], 0);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[3], 0);

            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[0], 0);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[1], 600);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[2], 0);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[3], 1800);
        }

        [Test]
        public async Task AreasReportByMonth_Generate_DoesGenerate()
        {
            Area newArea = new Area() { Name = "My Area 1", Version = 1 };
            Area newArea1 = new Area() { Name = "My Area 2", Version = 1 };
            Area newArea2 = new Area() { Name = "My Area 3", Version = 1 };
            Machine newMachine = new Machine() { Name = "My Machine 1", Version = 1 };
            Machine newMachine1 = new Machine() { Name = "My Machine 2", Version = 1 };
            Machine newMachine2 = new Machine() { Name = "My Machine 3", Version = 1 };

            await newArea.Create(DbContext);
            await newArea1.Create(DbContext);
            await newArea2.Create(DbContext);
            await newMachine.Create(DbContext);
            await newMachine1.Create(DbContext);
            await newMachine2.Create(DbContext);

            // Different Month
            MachineAreaTimeRegistration newTimeRegistrationMonth = new MachineAreaTimeRegistration()
            { AreaId = newArea.Id, Version = 1, SDKSiteId = 1, MachineId = newMachine.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 06, 13) };
            MachineAreaTimeRegistration newTimeRegistrationMonth1 = new MachineAreaTimeRegistration()
            { AreaId = newArea1.Id, Version = 1, SDKSiteId = 1, MachineId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 07, 14) };
            MachineAreaTimeRegistration newTimeRegistrationMonth2 = new MachineAreaTimeRegistration()
            { AreaId = newArea2.Id, Version = 1, SDKSiteId = 1, MachineId = newMachine2.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 07, 15) };
            MachineAreaTimeRegistration newTimeRegistrationMonth3 = new MachineAreaTimeRegistration()
            { AreaId = newArea.Id, Version = 1, SDKSiteId = 2, MachineId = newMachine.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 08, 13) };
            MachineAreaTimeRegistration newTimeRegistrationMonth4 = new MachineAreaTimeRegistration()
            { AreaId = newArea1.Id, Version = 1, SDKSiteId = 2, MachineId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 08, 14) };
            MachineAreaTimeRegistration newTimeRegistrationMonth5 = new MachineAreaTimeRegistration()
            { AreaId = newArea1.Id, Version = 1, SDKSiteId = 2, MachineId = newMachine2.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 09, 15) };
            MachineAreaTimeRegistration newTimeRegistrationMonth6 = new MachineAreaTimeRegistration()
            { AreaId = newArea1.Id, Version = 1, SDKSiteId = 2, MachineId = newMachine1.Id, TimeInSeconds = 36000, DoneAt = new DateTime(2019, 09, 15) };

            await DbContext.MachineAreaTimeRegistrations.AddRangeAsync(newTimeRegistrationMonth, newTimeRegistrationMonth1,
                newTimeRegistrationMonth2, newTimeRegistrationMonth3, newTimeRegistrationMonth4, newTimeRegistrationMonth5,
                newTimeRegistrationMonth6);

            await DbContext.SaveChangesAsync();

            GenerateReportModel model = new GenerateReportModel()
            {
                DateFrom = new DateTime(2019, 5, 13),
                DateTo = new DateTime(2019, 9, 13),
                Relationship = ReportRelationshipType.Area,
                Type = ReportType.Month
            };

            List<Site_Dto> sitesList = new List<Site_Dto>()
            {
                new Site_Dto(1, "Test Site 1", "", "", 1, 1, 1, 1),
                new Site_Dto(2, "Test Site 2", "", "", 1, 1, 1, 1)
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

            List<MachineAreaTimeRegistration> jobsList = await DbContext.MachineAreaTimeRegistrations
                .Include(x => x.Machine)
                .Include(x => x.Area)
                .Where(x => x.DoneAt >= modelDateFrom && x.DoneAt <= modelDateTo)
                .ToListAsync();

            ReportModel reportModel = ReportsHelper.GetReportData(model, jobsList, sitesList, (int)ReportTimeType.Minutes);

            Assert.AreEqual(reportModel.SubReports.Count, 1);
            Assert.AreEqual(reportModel.SubReports[0].TotalTime, 3000);
            Assert.AreEqual(reportModel.SubReports[0].Entities.Count, 3);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].EntityName, newArea.Name);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].EntityName, newArea1.Name);
            Assert.AreEqual(reportModel.SubReports[0].Entities[2].EntityName, newArea2.Name);

            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TotalTime, 1200);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TotalTime, 1200);

            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[0], 0);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[1], 600);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[2], 0);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[3], 600);
            Assert.AreEqual(reportModel.SubReports[0].Entities[0].TimePerTimeUnit[4], 0);

            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[0], 0);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[1], 0);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[2], 600);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[3], 600);
            Assert.AreEqual(reportModel.SubReports[0].Entities[1].TimePerTimeUnit[4], 0);
        }
    }
}