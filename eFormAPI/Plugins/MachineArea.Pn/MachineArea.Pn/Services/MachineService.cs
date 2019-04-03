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
using MachineArea.Pn.Infrastructure.Models.Machines;
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
    public class MachineService : IMachineService
    {
        private readonly MachineAreaPnDbContext _dbContext;
        private readonly IMachineAreaLocalizationService _localizationService;
        private readonly ILogger<MachineService> _logger;
        private readonly IEFormCoreService _coreService;
        private readonly IBus _bus;

        public MachineService(MachineAreaPnDbContext dbContext,
            IMachineAreaLocalizationService localizationService,
            ILogger<MachineService> logger, 
            IEFormCoreService coreService, 
            IRebusService rebusService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
            _logger = logger;
            _coreService = coreService;
            _bus = rebusService.GetBus();
        }

        public async Task<OperationDataResult<MachinesModel>> GetAllMachines(MachineRequestModel requestModel)
        {
            try
            {
                var machinesModel = new MachinesModel();

                IQueryable<Machine> machinesQuery = _dbContext.Machines.AsQueryable();
                if (!string.IsNullOrEmpty(requestModel.Sort))
                {
                    if (requestModel.IsSortDsc)
                    {
                        machinesQuery = machinesQuery
                            .CustomOrderByDescending(requestModel.Sort);
                    }
                    else
                    {
                        machinesQuery = machinesQuery
                            .CustomOrderBy(requestModel.Sort);
                    }
                }
                else
                {
                    machinesQuery = _dbContext.Machines
                        .OrderBy(x => x.Id);
                }

                if (requestModel.PageSize != null)
                {
                    machinesQuery = machinesQuery.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                        .Skip(requestModel.Offset)
                        .Take((int)requestModel.PageSize);
                }

                var machines = await machinesQuery.Select(x => new MachineModel()
                {
                    Name = x.Name,
                    Id = x.Id
                }).ToListAsync();

                machinesModel.Total = await _dbContext.Machines.CountAsync();
                machinesModel.MachineList = machines;

                return new OperationDataResult<MachinesModel>(true, machinesModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<MachinesModel>(false,
                    _localizationService.GetString("ErrorObtainMachines"));
            }
        }

        public async Task<OperationDataResult<MachineModel>> GetSingleMachine(int machineId)
        {
            try
            {
                var machine = await _dbContext.Machines.Select(x => new MachineModel()
                    {
                        Name = x.Name,
                        Id = x.Id,
                        RelatedAreasIds = x.MachineAreas.Select(y => y.Area.Id).ToList()
                    })
                    .FirstOrDefaultAsync(x => x.Id == machineId);

                if (machine == null)
                {
                    return new OperationDataResult<MachineModel>(false,
                        _localizationService.GetString("MachineWithIdNotExist"));
                }

                return new OperationDataResult<MachineModel>(true, machine);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<MachineModel>(false,
                    _localizationService.GetString("ErrorObtainMachine"));
            }
        }

        public async Task<OperationResult> CreateMachine(MachineModel model)
        {
            try
            {                
                await model.Save(_dbContext);
                _bus.SendLocal(new MachineAreaCreate(model, null));
                return new OperationResult(true, _localizationService.GetString("MachineCreatedSuccesfully"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorCreatingMachine"));
            }
        }

        public async Task<OperationResult> UpdateMachine(MachineModel model)
        {
            try
            {
                await model.Update(_dbContext);
                _bus.SendLocal(new MachineAreaUpdate(model, null));
                return new OperationResult(true, _localizationService.GetString("MachineUpdatedSuccessfully"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorUpdatingMachine"));
            }
        }

        public async Task<OperationResult> DeleteMachine(int machineId)
        {
            try
            {
                MachineModel model = new MachineModel();
                model.Id = machineId;
                
                await model.Delete(_dbContext);
                _bus.SendLocal(new MachineAreaDelete(model, null));
                return new OperationResult(true, _localizationService.GetString("MachineDeletedSuccessfully"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorWhileDeletingMachine"));
            }
        }
    }
}
