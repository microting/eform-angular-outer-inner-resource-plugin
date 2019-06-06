using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using eFormShared;
using MachineArea.Pn.Abstractions;
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
                MachinesModel machinesModel = new MachinesModel();

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

                machinesQuery = machinesQuery.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed);

                if (requestModel.PageSize != null)
                {
                    machinesQuery = machinesQuery
                        .Skip(requestModel.Offset)
                        .Take((int)requestModel.PageSize);
                }

                List<MachineModel> machines = await machinesQuery.Select(x => new MachineModel()
                {
                    Name = x.Name,
                    Id = x.Id
                }).ToListAsync();

                machinesModel.Total = await _dbContext.Machines.CountAsync();
                machinesModel.MachineList = machines;
                
                try
                {
                    machinesModel.Name = _dbContext.PluginConfigurationValues.SingleOrDefault(x => x.Name == "MachineAreaBaseSettings:InnerResourceName").Value;  
                } catch {}

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
                MachineModel machine = await _dbContext.Machines.Select(x => new MachineModel()
                    {
                        Name = x.Name,
                        Id = x.Id
                    })
                    .FirstOrDefaultAsync(x => x.Id == machineId);
                                
                if (machine == null)
                {
                    return new OperationDataResult<MachineModel>(false,
                        _localizationService.GetString("MachineWithIdNotExist"));
                }
                
                List<Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea> machineAreas = _dbContext.MachineAreas
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed && x.MachineId == machine.Id).ToList();

                machine.RelatedAreasIds = new List<int>();
                foreach (Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea machineArea in machineAreas)
                {
                    machine.RelatedAreasIds.Add(machineArea.AreaId);
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
                List<Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea> machineAreas = model.RelatedAreasIds.Select(x =>
                    new Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea
                    {
                        Id = x
                    }).ToList();

                Machine newMachine = new Machine()
                {
                    Name = model.Name,
                    MachineAreas = machineAreas
                };

                await newMachine.Save(_dbContext);
                model.Id = newMachine.Id;
                await _bus.SendLocal(new MachineAreaCreate(model, null));
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
                List<Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea> machineAreas = model.RelatedAreasIds.Select(x =>
                    new Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea
                    {
                        Id = x
                    }).ToList();

                Machine selectedMachine = new Machine()
                {
                    Name = model.Name,
                    MachineAreas = machineAreas,
                    Id = model.Id
                };

                await selectedMachine.Update(_dbContext);
                await _bus.SendLocal(new MachineAreaUpdate(model, null));
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
                Machine selectedMachine = new Machine()
                {
                    Id = machineId
                };
                
                await selectedMachine.Delete(_dbContext);
                await _bus.SendLocal(new MachineAreaDelete(new MachineModel() { Id = machineId }, null));
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
