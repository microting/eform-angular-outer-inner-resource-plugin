namespace OuterInnerResource.Pn.Infrastructure.Models.OuterResources
{
    public class OuterResourceRequestModel
    {
        public string Sort { get; set; }
        public int PageIndex { get; set; }
        public int Offset { get; set; }
        public bool IsSortDsc { get; set; }
        public int? PageSize { get; set; }
    }
}