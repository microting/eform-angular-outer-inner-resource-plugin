using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.Report
{
    public class ReportEntityModel
    {
        public decimal TotalTime { get; set; }
        public List<decimal> TimePerTimeUnit { get; set; }
        public string EntityName { get; set; }
        public string RelatedEntityName { get; set; }
        public int? RelatedEntityId { get; set; }
        public int EntityId { get; set; }
    }
}
