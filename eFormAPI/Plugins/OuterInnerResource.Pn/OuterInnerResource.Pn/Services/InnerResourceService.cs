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
using OuterInnerResource.Pn.Infrastructure.Models.InnerResources;
using OuterInnerResource.Pn.Messages;
using Rebus.Bus;

namespace OuterInnerResource.Pn.Services
{
    public class InnerResourceService : IInnerResourceService
    {
        private readonly OuterInnerResourcePnDbContext _dbContext;
        private readonly IOuterInnerResourceLocalizationService _localizationService;
        private readonly ILogger<InnerResourceService> _logger;
        private readonly IEFormCoreService _coreService;
        private readonly IBus _bus;

        public InnerResourceService(OuterInnerResourcePnDbContext dbContext,
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

        public async Task<OperationDataResult<InnerResourcesModel>> Index(InnerResourceRequestModel requestModel)
        {
            try
            {
                InnerResourcesModel innerResourcesModel = new InnerResourcesModel();

                IQueryable<InnerResource> query = _dbContext.InnerResources.AsQueryable();
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
                    query = _dbContext.InnerResources
                        .OrderBy(x => x.Id);
                }

                query = query.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed);

                if (requestModel.PageSize != null)
                {
                    query = query
                        .Skip(requestModel.Offset)
                        .Take((int)requestModel.PageSize);
                }

                List<InnerResourceModel> innerResourceList = await query.Select(x => new InnerResourceModel()
                {
                    Name = x.Name,
                    Id = x.Id,
                    ExternalId = x.ExternalId,
                    RelatedOuterResourcesIds = _dbContext.OuterInnerResources.AsNoTracking().Where(y => 
                        y.InnerResourceId == x.Id && y.WorkflowState != Constants.WorkflowStates.Removed).Select(z => z.OuterResourceId).ToList()
                }).AsNoTracking().ToListAsync();

                innerResourcesModel.Total = await _dbContext.InnerResources.AsNoTracking().Where(x => x.WorkflowState != Constants.WorkflowStates.Removed).CountAsync();
                innerResourcesModel.InnerResourceList = innerResourceList;
                
                try
                {
                    innerResourcesModel.Name = _dbContext.PluginConfigurationValues.SingleOrDefault(x => 
                        x.Name == "OuterInnerResourceSettings:InnerResourceName")
                        ?.Value;  
                } catch {}

                return new OperationDataResult<InnerResourcesModel>(true, innerResourcesModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<InnerResourcesModel>(false,
                    _localizationService.GetString("ErrorObtainInnerResources"));
            }
        }

        public async Task<OperationDataResult<InnerResourceModel>> Get(int innerResourceId)
        {
            try
            {
                InnerResourceModel innerResource = await _dbContext.InnerResources.Select(x => new InnerResourceModel()
                    {
                        Name = x.Name,
                        Id = x.Id,
                        ExternalId = x.ExternalId
                    })
                    .FirstOrDefaultAsync(x => x.Id == innerResourceId);
                                
                if (innerResource == null)
                {
                    return new OperationDataResult<InnerResourceModel>(false,
                        _localizationService.GetString("InnerResourceWithIdNotExist"));
                }
                
                List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> outerInnerResources = await _dbContext.OuterInnerResources
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed && x.InnerResourceId == innerResource.Id).AsNoTracking().ToListAsync();

                innerResource.RelatedOuterResourcesIds = new List<int>();
                foreach (Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource outerInnerResource in outerInnerResources)
                {
                    innerResource.RelatedOuterResourcesIds.Add(outerInnerResource.OuterResourceId);
                }
                
                return new OperationDataResult<InnerResourceModel>(true, innerResource);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<InnerResourceModel>(false,
                    _localizationService.GetString("ErrorObtainInnerResource"));
            }
        }

        public async Task<OperationResult> Create(InnerResourceModel model)
        {
            try
            {
                List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> outerInnerResources 
                    = new List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource>();
                if (model.RelatedOuterResourcesIds != null)
                {
                    outerInnerResources = model.RelatedOuterResourcesIds.Select(x =>
                        new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource
                        {
                            Id = x
                        }).ToList();    
                }

                InnerResource innerResource = new InnerResource()
                {
                    Name = model.Name,
                    ExternalId = model.ExternalId,
                    OuterInnerResources = outerInnerResources
                };

                await innerResource.Create(_dbContext);
                model.Id = innerResource.Id;
                await _bus.SendLocal(new OuterInnerResourceCreate(model, null));

                return new OperationResult(true, 
                    _localizationService.GetString("InnerResourceCreatedSuccesfully", model.Name));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorCreatingInnerResource"));
            }
        }

        public async Task<OperationResult> Update(InnerResourceModel model)
        {
            try
            {
                List<Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource> outerInnerResources = model.RelatedOuterResourcesIds.Select(x =>
                    new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource
                    {
                        Id = x
                    }).ToList();

                InnerResource innerResource = new InnerResource()
                {
                    Name = model.Name,
                    ExternalId = model.ExternalId,
                    OuterInnerResources = outerInnerResources,
                    Id = model.Id
                };

                await innerResource.Update(_dbContext);
                await _bus.SendLocal(new OuterInnerResourceUpdate(model, null));
                return new OperationResult(true, _localizationService.GetString("InnerResourceUpdatedSuccessfully"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorUpdatingInnerResource"));
            }
        }

        public async Task<OperationResult> Delete(int innerResourceId)
        {
            try
            {
                InnerResource innerResource = new InnerResource()
                {
                    Id = innerResourceId
                };
                
                await innerResource.Delete(_dbContext);
                await _bus.SendLocal(new OuterInnerResourceDelete(new InnerResourceModel() { Id = innerResourceId }, null));
                return new OperationResult(true, _localizationService.GetString("InnerResourceDeletedSuccessfully"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorWhileDeletingInnerResource"));
            }
        }
    }
}
