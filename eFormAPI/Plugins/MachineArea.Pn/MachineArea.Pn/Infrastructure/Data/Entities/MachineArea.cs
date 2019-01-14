using Microting.eFormApi.BasePn.Infrastructure.Database.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace MachineArea.Pn.Infrastructure.Data.Entities
{
    public class MachineArea : BaseEntity
    {
        public int MachineId { get; set; }
        public virtual Machine Machine { get; set; }
        public int AreaId { get; set; }
        public virtual Area Area { get; set; }
        public int MicrotingeFormSdkId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [StringLength(255)]
        public string WorkflowState { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
    }
}