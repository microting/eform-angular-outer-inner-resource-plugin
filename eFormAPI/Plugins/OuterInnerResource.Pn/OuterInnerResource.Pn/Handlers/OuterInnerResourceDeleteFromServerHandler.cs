using System.Threading.Tasks;
using eFormCore;
using Microsoft.EntityFrameworkCore;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using OuterInnerResource.Pn.Infrastructure.Helpers;
using OuterInnerResource.Pn.Messages;
using Rebus.Handlers;

namespace OuterInnerResource.Pn.Handlers
{
    public class OuterInnerResourceDeleteFromServerHandler : IHandleMessages<OuterInnerResourceDeleteFromServer>
    {
        private readonly Core _core;
        private readonly OuterInnerResourcePnDbContext _dbContext;
        
        public OuterInnerResourceDeleteFromServerHandler(Core core, DbContextHelper dbContextHelper)
        {
            _core = core;
            _dbContext = dbContextHelper.GetDbContext();
        }
        
        public async Task Handle(OuterInnerResourceDeleteFromServer message)
        {
            OuterInnerResourceSite outerInnerResourceSite =
                await _dbContext.OuterInnerResourceSites.SingleOrDefaultAsync(x =>
                    x.Id == message.OuterInnerResourceSiteId);
            if (outerInnerResourceSite.MicrotingSdkCaseId != null)
            {
                await _core.CaseDelete((int) outerInnerResourceSite.MicrotingSdkCaseId);
//                if (result)
//                {
//                    await outerInnerResourceSite.Delete(_dbContext);
//                }    
            }
        }
    }
}