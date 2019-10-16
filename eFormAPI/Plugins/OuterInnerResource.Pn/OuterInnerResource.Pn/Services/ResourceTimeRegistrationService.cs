using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
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
            
            return new OperationDataResult<ResourceTimeRegistrationsModel>(true, resourceTimeRegistrationsModel);
        }
    }
}