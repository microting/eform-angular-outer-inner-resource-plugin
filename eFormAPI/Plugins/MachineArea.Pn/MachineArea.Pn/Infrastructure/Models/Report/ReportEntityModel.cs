using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.Report
{
    public class ReportEntityModel
    {
        public decimal TotalTime { get; set; }
        public List<decimal> TimePerTimeUnit { get; set; }
        public string EntityName { get; set; }
        public int EntityId { get; set; }
    }
}
