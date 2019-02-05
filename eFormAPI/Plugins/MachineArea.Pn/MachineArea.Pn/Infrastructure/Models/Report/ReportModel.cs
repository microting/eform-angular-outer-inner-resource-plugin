using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.Report
{
    public class ReportModel
    {
        public List<ReportEntityHeaderModel> ReportHeaders { get; set; }
        public List<ReportEntityModel> Entities { get; set; }
        public decimal TotalTime { get; set; }
        public List<decimal> TotalTimePerTimeUnit { get; set; }
    }
}
