using System;
using System.Collections.Generic;
using System.Text;
using eFormData;

namespace MachineArea.Pn.Infrastructure.Models.Report
{
    public class GenerateReportModel
    {
        public int Type { get; set; }
        public int Relationship { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
