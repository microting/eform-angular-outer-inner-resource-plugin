using System;
using OuterInnerResource.Pn.Infrastructure.Enums;

namespace OuterInnerResource.Pn.Infrastructure.Models.Report
{
    public class GenerateReportModel
    {
        public ReportType Type { get; set; }
        public ReportRelationshipType Relationship { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
