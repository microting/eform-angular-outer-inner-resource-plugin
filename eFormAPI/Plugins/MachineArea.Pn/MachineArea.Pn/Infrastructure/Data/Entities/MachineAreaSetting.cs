using Microting.eFormApi.BasePn.Infrastructure.Database.Base;


namespace MachineArea.Pn.Infrastructure.Data.Entities
{
    public class MachineAreaSetting : BaseEntity
    {
        public int? SelectedTemplateId { get; set; }
        public string SelectedTemplateName { get; set; }
        public int? RelatedEntityGroupId { get; set; }
    }
}