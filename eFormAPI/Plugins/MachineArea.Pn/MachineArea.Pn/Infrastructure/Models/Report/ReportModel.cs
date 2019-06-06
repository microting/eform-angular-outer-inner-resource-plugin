using System.Collections.Generic;
using MachineArea.Pn.Infrastructure.Enums;

namespace MachineArea.Pn.Infrastructure.Models.Report
{
    public class ReportModel
    {
        public List<ReportEntityHeaderModel> ReportHeaders { get; set; }
        public List<SubReportModel> SubReports { get; set; } = new List<SubReportModel>();
        public ReportRelationshipType Relationship { get; set; }
        public string HumanReadableName { get; set; }
    }
}
