using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using eFormShared;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Data;
using MachineArea.Pn.Infrastructure.Data.Entities;
using MachineArea.Pn.Infrastructure.Models.Machines;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Infrastructure.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Services
{
    public class MachineService : IMachineService
    {
        private readonly MachineAreaPnDbContext _dbContext;
        private readonly IMachineAreaLocalizationService _localizationService;
        private readonly ILogger<MachineService> _logger;

        public MachineService(MachineAreaPnDbContext dbContext,
            IMachineAreaLocalizationService localizationService,
            ILogger<MachineService> logger)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
            _logger = logger;
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
                    machinesQuery = machinesQuery
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
                    _localizationService.GetString(""));
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
                        _localizationService.GetString(""));
                }

                return new OperationDataResult<MachineModel>(true, machine);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<MachineModel>(false,
                    _localizationService.GetString(""));
            }
        }

        public async Task<OperationResult> CreateMachine(MachineCreateModel model)
        {
            try
            {
                var newMachine = new Machine()
                {
                    Name = model.Name,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = 1,
                    UpdatedByUserId = 2,
                    UpdatedAt = DateTime.UtcNow,
                    WorkflowState = Constants.WorkflowStates.Created,
                    MachineAreas = model.RelatedAreasIds
                        .Select(x => new Infrastructure.Data.Entities.MachineArea()
                        {
                            AreaId = x
                        }).ToList()
                };

                await _dbContext.Machines.AddAsync(newMachine);
                await _dbContext.SaveChangesAsync();
                return new OperationResult(true, _localizationService.GetString(""));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString(""));
            }
        }

        public async Task<OperationResult> UpdateMachine(MachineUpdateModel model)
        {
            try
            {
                var machineForUpdate = await _dbContext.Machines.FirstOrDefaultAsync(x => x.Id == model.Id);

                if (machineForUpdate == null)
                {
                    return new OperationResult(false,
                        _localizationService.GetString(""));
                }

                machineForUpdate.Name = model.Name;
                machineForUpdate.WorkflowState = Constants.WorkflowStates.Processed;
                machineForUpdate.UpdatedByUserId = 2;
                machineForUpdate.UpdatedAt = DateTime.UtcNow;

                var machinesForDelete = await _dbContext.MachineAreas
                    .Where(x => !model.RelateAreasIds.Contains(x.AreaId) && x.MachineId == model.Id)
                    .ToListAsync();

                var areaIds = await _dbContext.MachineAreas
                    .Where(x => model.RelateAreasIds.Contains(x.AreaId) && x.MachineId == model.Id)
                    .Select(x => x.AreaId)
                    .ToListAsync();

                _dbContext.RemoveRange(machinesForDelete);

                foreach (var areaId in model.RelateAreasIds)
                {
                    if (!areaIds.Contains(areaId))
                    {
                        machineForUpdate.MachineAreas.Add(new Infrastructure.Data.Entities.MachineArea()
                        {
                            AreaId = areaId,
                            MachineId = model.Id
                        });
                    }
                }

                await _dbContext.SaveChangesAsync();
                return new OperationResult(true, _localizationService.GetString(""));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString(""));
            }
        }

        public async Task<OperationResult> DeleteMachine(int machineId)
        {
            try
            {
                var machineForDelete = await _dbContext.Machines.FirstOrDefaultAsync(x => x.Id == machineId);

                if (machineForDelete == null)
                {
                    return new OperationResult(false,
                        _localizationService.GetString(""));
                }

                _dbContext.Machines.Remove(machineForDelete);
                await _dbContext.SaveChangesAsync();
                return new OperationResult(true, _localizationService.GetString(""));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString(""));
            }
        }
    }
}
