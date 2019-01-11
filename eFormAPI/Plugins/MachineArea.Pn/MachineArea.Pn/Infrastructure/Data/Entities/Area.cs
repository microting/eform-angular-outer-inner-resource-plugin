using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microting.eFormApi.BasePn.Infrastructure.Database.Base;

namespace MachineArea.Pn.Infrastructure.Data.Entities
{
    public class Area : BaseEntity
    {
        public Area()
        {
            this.MachineAreas = new HashSet<MachineArea>();
        }

        [StringLength(250)]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [StringLength(255)]
        public string WorkflowState { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public virtual ICollection<MachineArea> MachineAreas { get; set; }
    }
}