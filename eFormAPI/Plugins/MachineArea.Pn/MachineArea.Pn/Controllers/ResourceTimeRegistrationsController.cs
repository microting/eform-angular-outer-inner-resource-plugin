using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.ResourceTimeRegistrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Controllers
{
    [Authorize]
    public class ResourceTimeRegistrationsController : Controller
    {
        private readonly IResourceTimeRegistrationService _resourceTimeRegistrationService;
        
        public ResourceTimeRegistrationsController(IResourceTimeRegistrationService resourceTimeRegistrationService)
        {
            _resourceTimeRegistrationService = resourceTimeRegistrationService;
        }
        
        [HttpGet]
        [Route("api/outer-inner-resource-pn/resource-time-registrations/{lastRegistrationId}")]
        public async Task<OperationDataResult<ResourceTimeRegistrationsModel>> GetAllAreas(int lastRegistrationId)
        {
            return await _resourceTimeRegistrationService.GetAllRegistrations(lastRegistrationId);
        }
    }
}