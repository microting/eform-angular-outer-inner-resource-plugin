using System;
using System.Collections.Generic;
using System.Text;

namespace MachineArea.Pn.Infrastructure.Models.Report
{
    public class ReportModel
    {
        public List<ReportEntityHeaderModel> ReportHeaders { get; set; }
        public List<ReportEntityModel> Entities { get; set; }
        public int TotalTime { get; set; }
        public List<int> TotalTimePerTimeUnit { get; set; }
    }
}
