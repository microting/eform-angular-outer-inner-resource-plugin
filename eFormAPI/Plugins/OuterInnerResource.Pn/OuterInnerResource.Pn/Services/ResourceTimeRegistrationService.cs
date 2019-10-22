using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Models.ResourceTimeRegistrations;
using Rebus.Bus;

namespace OuterInnerResource.Pn.Services
{
    public class ResourceTimeRegistrationService : IResourceTimeRegistrationService
    {
        private readonly OuterInnerResourcePnDbContext _dbContext;
        private readonly IOuterInnerResourceLocalizationService _localizationService;
        private readonly ILogger<InnerResourceService> _logger;
        private readonly IEFormCoreService _coreService;
        private readonly IBus _bus;

        public ResourceTimeRegistrationService(OuterInnerResourcePnDbContext dbContext,
            IOuterInnerResourceLocalizationService localizationService,
            ILogger<InnerResourceService> logger, 
            IEFormCoreService coreService, 
            IRebusService rebusService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
            _logger = logger;
            _coreService = coreService;
            _bus = rebusService.GetBus();
        }
        
        public async Task<OperationDataResult<ResourceTimeRegistrationsModel>> GetAllRegistrations(int lastRegistrationId)
        {
            ResourceTimeRegistrationsModel resourceTimeRegistrationsModel = new ResourceTimeRegistrationsModel();
            resourceTimeRegistrationsModel.ResourceTimeRegistrationModels = new List<ResourceTimeRegistrationModel>();

            var results = _dbContext.ResourceTimeRegistrations.Where(x => x.Id > lastRegistrationId).Take(10).OrderBy(x => x.Id).ToList();
            foreach (ResourceTimeRegistration resourceTimeRegistration in results)
            {
                resourceTimeRegistrationsModel.ResourceTimeRegistrationModels.Add(new ResourceTimeRegistrationModel()
                {
                    DoneAt = resourceTimeRegistration.DoneAt,
                    DoneByDeviceUserId = resourceTimeRegistration.SDKSiteId,
                    DoneByDeviceUserName = "",
                    Id = resourceTimeRegistration.Id,
                    InnerResourceId = resourceTimeRegistration.InnerResourceId,
                    InnerResourceName = resourceTimeRegistration.InnerResource.Name,
                    OuterResourceId = resourceTimeRegistration.OuterResourceId,
                    OuterResourceName = resourceTimeRegistration.OuterResource.Name,
                    SdkCaseId = resourceTimeRegistration.SDKCaseId,
                    TimeInSeconds = resourceTimeRegistration.TimeInSeconds
                });
            }

            resourceTimeRegistrationsModel.LastResourceTimeRegistrationId = results.Last().Id;
            
            return new OperationDataResult<ResourceTimeRegistrationsModel>(true, resourceTimeRegistrationsModel);
        }
    }
}