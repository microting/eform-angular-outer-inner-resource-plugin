using Microting.eFormApi.BasePn.Infrastructure.Database.Base;

namespace MachineArea.Pn.Infrastructure.Data.Entities
{
    public class MachineArea : BaseEntity
    {
        public int MachineId { get; set; }
        public Machine Machine { get; set; }
        public int AreaId { get; set; }
        public Area Area { get; set; }
    }
}