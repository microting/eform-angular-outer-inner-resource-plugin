namespace OuterInnerResource.Pn.Messages
{        

    public class OuterInnerResourceDeleteFromServer
    {
        public int OuterInnerResourceSiteId { get; protected set; }

        public OuterInnerResourceDeleteFromServer(int outerInnerResourceSiteId)
        {
            OuterInnerResourceSiteId = outerInnerResourceSiteId;
        }
    }
}