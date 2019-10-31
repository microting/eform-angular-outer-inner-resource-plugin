using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
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
        private List<KeyValuePair<int, string>> _deviceUserNames;
        private List<KeyValuePair<int, string>> _outerResourceNames;
        private List<KeyValuePair<int, string>> _innerResourceNames;

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
            _deviceUserNames = new List<KeyValuePair<int, string>>();
            _outerResourceNames = new List<KeyValuePair<int, string>>();
            _innerResourceNames = new List<KeyValuePair<int, string>>();
        }
        
        public async Task<OperationDataResult<ResourceTimeRegistrationsModel>> GetAllRegistrations(int lastRegistrationId)
        {
            ResourceTimeRegistrationsModel resourceTimeRegistrationsModel = new ResourceTimeRegistrationsModel();
            resourceTimeRegistrationsModel.ResourceTimeRegistrationModels = new List<ResourceTimeRegistrationModel>();

            var results = _dbContext.ResourceTimeRegistrations.Where(x => x.Id > lastRegistrationId).Take(10).OrderBy(x => x.Id).ToList();
            foreach (ResourceTimeRegistration resourceTimeRegistration in results)
            {
                var registration = new ResourceTimeRegistrationModel()
                {
                    DoneAt = resourceTimeRegistration.DoneAt,
                    DoneByDeviceUserId = resourceTimeRegistration.SDKSiteId,
                    DoneByDeviceUserName = "",
                    Id = resourceTimeRegistration.Id,
                    InnerResourceId = resourceTimeRegistration.InnerResourceId,
                    InnerResourceName = "",
                    OuterResourceId = resourceTimeRegistration.OuterResourceId,
                    OuterResourceName = "",
                    SdkCaseId = resourceTimeRegistration.SDKCaseId,
                    TimeInSeconds = resourceTimeRegistration.TimeInSeconds
                };
                
                if (_deviceUserNames.Any(x => x.Key == registration.DoneByDeviceUserId))
                {
                    registration.DoneByDeviceUserName =
                        _deviceUserNames.First(x => x.Key == registration.DoneByDeviceUserId).Value;
                }
                else
                {
                    registration.DoneByDeviceUserName = _coreService.GetCore().Result.SiteRead(registration.DoneByDeviceUserId)?.Result.SiteName;
                    _deviceUserNames.Add(new KeyValuePair<int, string>(registration.DoneByDeviceUserId, registration.DoneByDeviceUserName));
                }

                if (_outerResourceNames.Any(x => x.Key == registration.OuterResourceId))
                {
                    registration.OuterResourceName =
                        _outerResourceNames.First(x => x.Key == registration.OuterResourceId).Value;
                }
                else
                {
                    registration.OuterResourceName =
                        _dbContext.OuterResources.First(x => x.Id == registration.OuterResourceId).Name;
                    _outerResourceNames.Add(new KeyValuePair<int, string>(registration.OuterResourceId, registration.OuterResourceName));
                }

                if (_innerResourceNames.Any(x => x.Key == registration.InnerResourceId))
                {
                    registration.InnerResourceName =
                        _innerResourceNames.First(x => x.Key == registration.InnerResourceId).Value;
                }
                else
                {
                    registration.InnerResourceName =
                        _dbContext.InnerResources.First(x => x.Id == registration.InnerResourceId).Name;
                    _innerResourceNames.Add(new KeyValuePair<int, string>(registration.InnerResourceId, registration.InnerResourceName));
                }
                resourceTimeRegistrationsModel.ResourceTimeRegistrationModels.Add(registration);
                
            }

            if (results.Count > 0)
            {
                resourceTimeRegistrationsModel.LastResourceTimeRegistrationId = results.Last().Id;
            }
            
            return new OperationDataResult<ResourceTimeRegistrationsModel>(true, resourceTimeRegistrationsModel);
        }
    }
}