using System.Collections.Generic;

namespace OuterInnerResource.Pn.Infrastructure.Models.Report
{
    public class SubReportModel
    {
        public List<ReportEntityModel> Entities { get; set; }
        public decimal TotalTime { get; set; }
        public List<decimal> TotalTimePerTimeUnit { get; set; }
    }
}
