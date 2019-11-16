using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eForm.Infrastructure.Constants;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Models.OuterResources;
using OuterInnerResource.Pn.Messages;
using Rebus.Bus;

namespace OuterInnerResource.Pn.Services
{
    public class OuterResourceService : IOuterResourceService
    {
        private readonly OuterInnerResourcePnDbContext _dbContext;
        private readonly IOuterInnerResourceLocalizationService _localizationService;
        private readonly ILogger<OuterResourceService> _logger;
        private readonly IEFormCoreService _coreService;
        private readonly IBus _bus;

        public OuterResourceService(OuterInnerResourcePnDbContext dbContext,
            IOuterInnerResourceLocalizationService localizationService,
            ILogger<OuterResourceService> logger, 
            IEFormCoreService coreService, 
            IRebusService rebusService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
            _logger = logger;
            _coreService = coreService;
            _bus = rebusService.GetBus();
        }

        public async Task<OperationDataResult<OuterResourcesModel>> GetAllAreas(OuterResourceRequestModel requestModel)
        {
            try
            {
                OuterResourcesModel outerResourcesModel = new OuterResourcesModel();

                IQueryable<OuterResource> areasQuery = _dbContext.OuterResources.AsQueryable();
                if (!string.IsNullOrEmpty(requestModel.Sort))
                {
                    if (requestModel.IsSortDsc)
                    {
                        areasQuery = areasQuery
                            .CustomOrderByDescending(requestModel.Sort);
                    }
                    else
                    {
                        areasQuery = areasQuery
                            .CustomOrderBy(requestModel.Sort);
                    }
                }
                else
                {
                    areasQuery = _dbContext.OuterResources
                        .OrderBy(x => x.Id);
                }

                areasQuery = areasQuery.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed);

                if (requestModel.PageSize != null)
                {
                    areasQuery = areasQuery
                        .Skip(requestModel.Offset)
                        .Take((int)requestModel.PageSize);
                }

                List<OuterResourceModel> areas = await areasQuery.Select(x => new OuterResourceModel()
                {
                    Name = x.Name,
                    Id = x.Id,
                    ExternalId = x.ExternalId,
                    RelatedInnerResourcesIds = _dbContext.OuterInnerResources.AsNoTracking().Where(y => 
                        y.OuterResourceId == x.Id).Select(z => z.InnerResourceId).ToList()
                }).AsNoTracking().ToListAsync();

                outerResourcesModel.Total = await _dbContext.OuterResources.CountAsync();
                outerResourcesModel.OuterResourceList = areas;
                
                try
                {
                    outerResourcesModel.Name = _dbContext.PluginConfigurationValues.SingleOrDefault(x => 
                        x.Name == "OuterInnerResourceSettings:OuterResourceName")
                        ?.Value;  
                } catch {}

                return new OperationDataResult<OuterResourcesModel>(true, outerResourcesModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<OuterResourcesModel>(false,
                    _localizationService.GetString("ErrorObtainAreas"));
            }
        }

        public async Task<OperationDataResult<OuterResourceModel>> GetSingleArea(int areaId)
        {
            try
            {
                OuterResourceModel outerResource = await _dbContext.OuterResources.Select(x => new OuterResourceModel()
                    {
                        Name = x.Name,
                        Id = x.Id,
                        ExternalId = x.ExternalId
                    })
                    .FirstOrDefaultAsync(x => x.Id == areaId);
                                
                if (outerResource == null)
                {
                    return new OperationDataResult<OuterResourceModel>(false,
                        _localizationService.GetString("AreaWithIdNotExist", areaId));
                }

                List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> machineAreas = _dbContext.OuterInnerResources
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed && x.OuterResourceId == outerResource.Id).ToList();

                outerResource.RelatedInnerResourcesIds = new List<int>();
                foreach (Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource machineArea in machineAreas)
                {
                    outerResource.RelatedInnerResourcesIds.Add(machineArea.InnerResourceId);
                }

                return new OperationDataResult<OuterResourceModel>(true, outerResource);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<OuterResourceModel>(false,
                    _localizationService.GetString("ErrorObtainArea"));
            }
        }

        public async Task<OperationResult> CreateArea(OuterResourceModel model)
        {
            try
            {
                List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> machineAreas 
                    = new List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource>();
                if (model.RelatedInnerResourcesIds != null)
                {
                    machineAreas = model.RelatedInnerResourcesIds.Select(x =>
                        new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource
                        {
                            Id = x
                        }).ToList();    
                }

                OuterResource newArea = new OuterResource()
                {
                    Name = model.Name,
                    OuterInnerResources = machineAreas,
                    ExternalId = model.ExternalId
                };

                await newArea.Create(_dbContext);
                model.Id = newArea.Id;
                await _bus.SendLocal(new OuterInnerResourceCreate(null, model));

                return new OperationResult(true, 
                    _localizationService.GetString("AreaCreatedSuccessfully", model.Name));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorCreatingArea"));
            }
        }

        public async Task<OperationResult> UpdateArea(OuterResourceModel model)
        {
            try
            {
                List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> machineAreas = model.RelatedInnerResourcesIds.Select(x =>
                    new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource
                    {
                        Id = x
                    }).ToList();

                OuterResource selectedArea = new OuterResource()
                {
                    Name = model.Name,
                    OuterInnerResources = machineAreas,
                    Id = model.Id,
                    ExternalId = model.ExternalId
                };

                await selectedArea.Update(_dbContext);
                await _bus.SendLocal(new OuterInnerResourceUpdate(null, model));
                return new OperationResult(true, _localizationService.GetString("AreaUpdatedSuccessfully"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorUpdatingArea"));
            }
        }

        public async Task<OperationResult> DeleteArea(int areaId)
        {
            try
            {
                OuterResource selectedArea = new OuterResource()
                {
                    Id = areaId
                };
                
                await selectedArea.Delete(_dbContext);
                await _bus.SendLocal(new OuterInnerResourceDelete(null, new OuterResourceModel() { Id = areaId }));
                return new OperationResult(true, _localizationService.GetString("AreaDeletedSuccessfully"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorWhileDeletingArea"));
            }
        }
    }
}
