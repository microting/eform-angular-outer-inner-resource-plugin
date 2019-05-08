using System.Collections.Generic;
using MachineArea.Pn.Infrastructure.Enums;

namespace MachineArea.Pn.Infrastructure.Models.Report
{
    public class ReportModel
    {
        public List<ReportEntityHeaderModel> ReportHeaders { get; set; }
        public List<ReportEntityModel> Entities { get; set; }
        public decimal TotalTime { get; set; }
        public ReportRelationshipType Relationship { get; set; }
        public List<decimal> TotalTimePerTimeUnit { get; set; }
    }
}
