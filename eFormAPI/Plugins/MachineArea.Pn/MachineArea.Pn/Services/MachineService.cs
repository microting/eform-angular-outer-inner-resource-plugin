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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

namespace MachineArea.Pn.Services
{
    public class MachineService : IMachineService
    {
        private readonly MachineAreaPnDbContext _dbContext;
        private readonly IMachineAreaLocalizationService _localizationService;
        private readonly ILogger<MachineService> _logger;
        private readonly IEFormCoreService _coreService;

        public MachineService(MachineAreaPnDbContext dbContext,
            IMachineAreaLocalizationService localizationService,
            ILogger<MachineService> logger, IEFormCoreService coreService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
            _logger = logger;
            _coreService = coreService;
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
                model.Save(_dbContext);
                string eFormId = _dbContext.MachineAreaSettings.FirstOrDefault(x => x.Name == MachineAreaSettingsModel.Settings.SdkeFormId.ToString()).Value;

                foreach (int areasId in model.RelatedAreasIds)
                {
                    
                }
//                var newMachine = new Machine()
//                {
//                    Name = model.Name,
//                    CreatedAt = DateTime.UtcNow,
//                    CreatedByUserId = 1,
//                    UpdatedByUserId = 2,
//                    UpdatedAt = DateTime.UtcNow,
//                    WorkflowState = Constants.WorkflowStates.Created,
//                    MachineAreas = model.RelatedAreasIds
//                        .Select(x => new Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea()
//                        {
//                            AreaId = x
//                        }).ToList()
//                };
                //model.save();

                // Obtaining SDK template for machine-area relationship
//                MachineAreaSetting machineAreaSettings = await _dbContext.MachineAreaSettings.FirstOrDefaultAsync();
//                if (machineAreaSettings.SelectedeFormId == null)
//                {
//                    return new OperationResult(false,
//                        _localizationService.GetString("ErrorCreatingMachine", model.Name));
//                }
//
//                Core core = _coreService.GetCore();
//
//                MainElement mainElement = core.TemplateRead((int)machineAreaSettings.SelectedeFormId);
//                List<Site_Dto> sites = core.SiteReadAll(false);
//
//                // Foreach machine-area binding pass sites and get id
//                //List<string> sdkIds = core.CaseCreate(mainElement, "", sites.Select(x => x.SiteId).ToList(), "");
//
//                // Foreach machine-area binding pass sites and get id
//                foreach (var newAreaMachineArea in newMachine.MachineAreas)
//                {
//                    newAreaMachineArea.MicrotingeFormSdkId = 
//                        int.Parse(core.CaseCreate(mainElement, "", sites.Select(x => x.SiteId).ToList(), "").FirstOrDefault());
//                }


                //await _dbContext.Machines.AddAsync(newMachine);
                //await _dbContext.SaveChangesAsync();
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
                string eFormId = _dbContext.MachineAreaSettings.FirstOrDefault(x => x.Name == MachineAreaSettingsModel.Settings.SdkeFormId.ToString()).Value;
                
//                if (machineAreaSettings.SelectedeFormId == null)
//                {
//                    return new OperationResult(false,
//                        _localizationService.GetString("ErrorUpdatingMachine"));
//                }
//
//                var machineForUpdate = await _dbContext.Machines.FirstOrDefaultAsync(x => x.Id == model.Id);
//                if (machineForUpdate == null)
//                {
//                    return new OperationResult(false,
//                        _localizationService.GetString("MachineWithIdNotExist", model.Id));
//                }
//
//                machineForUpdate.Name = model.Name;
//                machineForUpdate.WorkflowState = Constants.WorkflowStates.Processed;
//                machineForUpdate.UpdatedByUserId = 2;
//                machineForUpdate.UpdatedAt = DateTime.UtcNow;
//
//                var machinesForDelete = await _dbContext.MachineAreas
//                    .Where(x => !model.RelatedAreasIds.Contains(x.AreaId) && x.MachineId == model.Id)
//                    .ToListAsync();
//
//                var areaIds = await _dbContext.MachineAreas
//                    .Where(x => model.RelatedAreasIds.Contains(x.AreaId) && x.MachineId == model.Id)
//                    .Select(x => x.AreaId)
//                    .ToListAsync();
//
//                var core = _coreService.GetCore();
//                var mainElement = core.TemplateRead((int)machineAreaSettings.SelectedeFormId);
//                var sites = core.SiteReadAll(false);
//                // delete cases
//                foreach (var machineForDelete in machinesForDelete)
//                {
//                    core.CaseDelete(
//                        (int)machineAreaSettings.SelectedeFormId,
//                        machineForDelete.MicrotingeFormSdkId);
//                }
//
//                _dbContext.RemoveRange(machinesForDelete);
//
//                foreach (var areaId in model.RelatedAreasIds)
//                {
//                    if (!areaIds.Contains(areaId))
//                    {
//                        var newAreaMachineArea =
//                            new Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea()
//                            {
//                                AreaId = areaId,
//                                MachineId = model.Id
//                            };
//                        // Add case
//                        var caseId = core.CaseCreate(mainElement, "", sites.Select(x => x.SiteId).ToList(), "")
//                            .FirstOrDefault();
//                        newAreaMachineArea.MicrotingeFormSdkId = int.Parse(caseId);
//                        machineForUpdate.MachineAreas.Add(newAreaMachineArea);
//                    }
//                }

                await _dbContext.SaveChangesAsync();
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
                // Obtaining SDK template for machine-area relationship
//                MachineAreaSetting machineAreaSettings = await _dbContext.MachineAreaSettings.FirstOrDefaultAsync();
//                if (machineAreaSettings.SelectedeFormId == null)
//                {
//                    return new OperationResult(false,
//                        _localizationService.GetString("ErrorWhileDeletingMachine"));
//                }
//
//                var machineForDelete = await _dbContext.Machines.FirstOrDefaultAsync(x => x.Id == machineId);
//                if (machineForDelete == null)
//                {
//                    return new OperationResult(false,
//                        _localizationService.GetString("MachineWithIdNotExist"));
//                }
//
//                var machineAreasForDelete = await _dbContext.MachineAreas
//                    .Where(x => x.MachineId == machineId)
//                    .ToListAsync();
//
//                Core core = _coreService.GetCore();
//
//                // Removing cases
//                foreach (var machineArea in machineAreasForDelete)
//                {
//                    core.CaseDelete((int) machineAreaSettings.SelectedeFormId, machineArea.MicrotingeFormSdkId);
//                }

                //_dbContext.Machines.Remove(machineForDelete);
                await _dbContext.SaveChangesAsync();
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
