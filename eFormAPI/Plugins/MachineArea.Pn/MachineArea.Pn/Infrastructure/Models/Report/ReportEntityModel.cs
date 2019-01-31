using System;
using System.Collections.Generic;
using System.Text;

namespace MachineArea.Pn.Infrastructure.Models.Report
{
    public class ReportEntityModel
    {
        public int TotalTime { get; set; }
        public List<int> TimePerTimeUnit { get; set; }
        public string EntityName { get; set; }
        public int EntityId { get; set; }
    }
}
