using System;
using System.ComponentModel.DataAnnotations;

namespace MachineArea.Pn.Infrastructure.Models
{
    [Obsolete]
    public class MachineAreaSettingModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [StringLength(255)] public string WorkflowState { get; set; }
        public int Version { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public string Name { get; set; }
    }
}