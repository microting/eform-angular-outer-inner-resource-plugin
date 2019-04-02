using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using eFormCore;
using eFormData;
using eFormShared;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models;
using MachineArea.Pn.Infrastructure.Models.Areas;
using MachineArea.Pn.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;
using Rebus.Bus;

namespace MachineArea.Pn.Services
{
    public class AreaService : IAreaService
    {
        private readonly MachineAreaPnDbContext _dbContext;
        private readonly IMachineAreaLocalizationService _localizationService;
        private readonly ILogger<AreaService> _logger;
        private readonly IEFormCoreService _coreService;
        private readonly IBus _bus;

        public AreaService(MachineAreaPnDbContext dbContext,
            IMachineAreaLocalizationService localizationService,
            ILogger<AreaService> logger, 
            IEFormCoreService coreService, 
            IRebusService rebusService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
            _logger = logger;
            _coreService = coreService;
            _bus = rebusService.GetBus();
        }

        public async Task<OperationDataResult<AreasModel>> GetAllAreas(AreaRequestModel requestModel)
        {
            try
            {
                var areasModel = new AreasModel();

                IQueryable<Area> areasQuery = _dbContext.Areas.AsQueryable();
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
                    areasQuery = _dbContext.Areas
                        .OrderBy(x => x.Id);
                }

                if (requestModel.PageSize != null)
                {
                    areasQuery = areasQuery.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                        .Skip(requestModel.Offset)
                        .Take((int)requestModel.PageSize);
                }

                var areas = await areasQuery.Select(x => new AreaModel()
                {
                    Name = x.Name,
                    Id = x.Id
                }).ToListAsync();

                areasModel.Total = await _dbContext.Areas.CountAsync();
                areasModel.AreaList = areas;

                return new OperationDataResult<AreasModel>(true, areasModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<AreasModel>(false,
                    _localizationService.GetString("ErrorObtainAreas"));
            }
        }

        public async Task<OperationDataResult<AreaModel>> GetSingleArea(int areaId)
        {
            try
            {
                AreaModel area = await _dbContext.Areas.Select(x => new AreaModel()
                    {
                        Name = x.Name,
                        Id = x.Id,
                        RelatedMachinesIds = x.MachineAreas.Select(y => y.Machine.Id).ToList()
                    })
                    .FirstOrDefaultAsync(x => x.Id == areaId);

                if (area == null)
                {
                    return new OperationDataResult<AreaModel>(false,
                        _localizationService.GetString("AreaWithIdNotExist", areaId));
                }

                return new OperationDataResult<AreaModel>(true, area);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<AreaModel>(false,
                    _localizationService.GetString("ErrorObtainArea"));
            }
        }

        public async Task<OperationResult> CreateArea(AreaModel model)
        {
            try
            {
                await model.Save(_dbContext);
                _bus.SendLocal(new MachineAreaCreate(null, model));
                return new OperationResult(true, 
                    _localizationService.GetString("AreaCreatedSuccesfully", model.Name));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorCreatingArea"));
            }
        }

        public async Task<OperationResult> UpdateArea(AreaModel model)
        {
            try
            {
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
