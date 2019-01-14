using Microting.eFormApi.BasePn.Infrastructure.Database.Base;


namespace MachineArea.Pn.Infrastructure.Data.Entities
{
    public class MachineAreaSetting : BaseEntity
    {
        public int? SelectedeFormId { get; set; }
        public string SelectedeFormName { get; set; }
    }
}