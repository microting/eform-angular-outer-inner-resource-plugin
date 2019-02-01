using System;
using MachineArea.Pn.Infrastructure.Enums;

namespace MachineArea.Pn.Infrastructure.Models.Report
{
    public class GenerateReportModel
    {
        public ReportType Type { get; set; }
        public int Relationship { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
