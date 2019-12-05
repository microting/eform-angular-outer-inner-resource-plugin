namespace OuterInnerResource.Pn.Messages
{
    public class OuterInnerResourcePosteForm
    {
        public int OuterInnerResourceSiteId { get; protected set; }
        public int SdkeFormId { get; protected set; }

        public OuterInnerResourcePosteForm(int outerInnerResourceSiteId, int sdkeFormId)
        {
            OuterInnerResourceSiteId = outerInnerResourceSiteId;
            SdkeFormId = sdkeFormId;
        }
    }
}