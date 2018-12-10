namespace MachineArea.Pn.Infrastructure.Models.Areas
{
    public class AreaRequestModel
    {
        public string Sort { get; set; }
        public int PageIndex { get; set; }
        public int Offset { get; set; }
        public bool IsSortDsc { get; set; }
        public int? PageSize { get; set; }
    }
}