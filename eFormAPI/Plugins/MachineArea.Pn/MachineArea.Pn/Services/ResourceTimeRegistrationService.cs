using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.ResourceTimeRegistrations;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Rebus.Bus;

namespace MachineArea.Pn.Services
{
    public class ResourceTimeRegistrationService : IResourceTimeRegistrationService
    {
        private readonly OuterInnerResourcePnDbContext _dbContext;
        private readonly IMachineAreaLocalizationService _localizationService;
        private readonly ILogger<InnerResourceService> _logger;
        private readonly IEFormCoreService _coreService;
        private readonly IBus _bus;

        public ResourceTimeRegistrationService(OuterInnerResourcePnDbContext dbContext,
            IMachineAreaLocalizationService localizationService,
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