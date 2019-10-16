using System.Collections.Generic;
using OuterInnerResource.Pn.Infrastructure.Enums;

namespace OuterInnerResource.Pn.Infrastructure.Models.Report
{
    public class ReportModel
    {
        public List<ReportEntityHeaderModel> ReportHeaders { get; set; }
        public List<SubReportModel> SubReports { get; set; } = new List<SubReportModel>();
        public ReportRelationshipType Relationship { get; set; }
        public string HumanReadableName { get; set; }
    }
}
