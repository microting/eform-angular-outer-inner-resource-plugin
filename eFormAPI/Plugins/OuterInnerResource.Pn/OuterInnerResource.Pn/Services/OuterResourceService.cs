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

        public async Task<OperationDataResult<OuterResourcesModel>> Index(OuterResourceRequestModel requestModel)
        {
            try
            {
                OuterResourcesModel outerResourcesModel = new OuterResourcesModel();

                IQueryable<OuterResource> query = _dbContext.OuterResources.AsQueryable();
                if (!string.IsNullOrEmpty(requestModel.Sort))
                {
                    if (requestModel.IsSortDsc)
                    {
                        query = query
                            .CustomOrderByDescending(requestModel.Sort);
                    }
                    else
                    {
                        query = query
                            .CustomOrderBy(requestModel.Sort);
                    }
                }
                else
                {
                    query = _dbContext.OuterResources
                        .OrderBy(x => x.Id);
                }

                query = query.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed);

                if (requestModel.PageSize != null)
                {
                    query = query
                        .Skip(requestModel.Offset)
                        .Take((int)requestModel.PageSize);
                }

                List<OuterResourceModel> outerResourceList = await query.Select(x => new OuterResourceModel()
                {
                    Name = x.Name,
                    Id = x.Id,
                    ExternalId = x.ExternalId,
                    RelatedInnerResourcesIds = _dbContext.OuterInnerResources.AsNoTracking().Where(y => 
                        y.OuterResourceId == x.Id && y.WorkflowState != Constants.WorkflowStates.Removed).Select(z => z.InnerResourceId).ToList()
                }).AsNoTracking().ToListAsync();

                outerResourcesModel.Total = await _dbContext.OuterResources.AsNoTracking().Where(x => x.WorkflowState != Constants.WorkflowStates.Removed).CountAsync();
                outerResourcesModel.OuterResourceList = outerResourceList;
                
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

        public async Task<OperationDataResult<OuterResourceModel>> Get(int areaId)
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

                List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> outerInnerResources = await _dbContext.OuterInnerResources
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed && x.OuterResourceId == outerResource.Id).AsNoTracking().ToListAsync();

                outerResource.RelatedInnerResourcesIds = new List<int>();
                foreach (Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource outerInnerResource in outerInnerResources)
                {
                    outerResource.RelatedInnerResourcesIds.Add(outerInnerResource.InnerResourceId);
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

        public async Task<OperationResult> Create(OuterResourceModel model)
        {
            try
            {
                List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> outerInnerResources 
                    = new List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource>();
//                if (model.RelatedInnerResourcesIds != null)
//                {
//                    outerInnerResources = model.RelatedInnerResourcesIds.Select(x =>
//                        new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource
//                        {
//                            Id = x
//                        }).ToList();    
//                }

                OuterResource outerResource = new OuterResource()
                {
                    Name = model.Name,
//                    OuterInnerResources = outerInnerResources,
                    ExternalId = model.ExternalId
                };

                await outerResource.Create(_dbContext);
                model.Id = outerResource.Id;
                if (model.RelatedInnerResourcesIds != null)
                {
                    foreach (var innerResourceId in model.RelatedInnerResourcesIds)
                    {
                        var macth = await _dbContext.OuterInnerResources.SingleOrDefaultAsync(x =>
                            x.OuterResourceId == model.Id
                            && x.InnerResourceId == innerResourceId);
                        if (macth == null)
                        {
                            Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource
                                outerInnerResource =
                                    new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.
                                        OuterInnerResource
                                        {
                                            InnerResourceId = innerResourceId, 
                                            OuterResourceId = model.Id
                                        };
                            await outerInnerResource.Create(_dbContext);
                        }
                    }
                    await _bus.SendLocal(new OuterInnerResourceCreate(null, model));
                }
//                await _bus.SendLocal(new OuterInnerResourceCreate(null, model));

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

        public async Task<OperationResult> Update(OuterResourceModel model)
        {
            try
            {
                List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> outerInnerResources = model.RelatedInnerResourcesIds.Select(x =>
                    new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource
                    {
                        Id = x
                    }).ToList();

                OuterResource outerResource = new OuterResource()
                {
                    Name = model.Name,
                    OuterInnerResources = outerInnerResources,
                    Id = model.Id,
                    ExternalId = model.ExternalId
                };

                await outerResource.Update(_dbContext);
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

        public async Task<OperationResult> Delete(int outerResourceId)
        {
            try
            {
                OuterResource outerResource = new OuterResource()
                {
                    Id = outerResourceId
                };
                
                await outerResource.Delete(_dbContext);
                await _bus.SendLocal(new OuterInnerResourceDelete(null, new OuterResourceModel() { Id = outerResourceId }));
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
