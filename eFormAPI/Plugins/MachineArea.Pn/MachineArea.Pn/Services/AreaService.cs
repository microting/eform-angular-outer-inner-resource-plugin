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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

namespace MachineArea.Pn.Services
{
    public class AreaService : IAreaService
    {
        private readonly MachineAreaPnDbContext _dbContext;
        private readonly IMachineAreaLocalizationService _localizationService;
        private readonly ILogger<AreaService> _logger;
        private readonly IEFormCoreService _coreService;

        public AreaService(MachineAreaPnDbContext dbContext,
            IMachineAreaLocalizationService localizationService,
            ILogger<AreaService> logger, IEFormCoreService coreService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
            _logger = logger;
            _coreService = coreService;
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

        public async Task<OperationResult> CreateArea(AreaCreateModel model)
        {
            try
            {
                var newArea = new Area()
                {
                    Name = model.Name,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = 1,
                    UpdatedByUserId = 2,
                    UpdatedAt = DateTime.UtcNow,
                    WorkflowState = Constants.WorkflowStates.Created,
                    MachineAreas = model.RelatedMachinesIds
                        .Select(x => new Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea()
                        {
                            MachineId = x
                        }).ToList()
                };

                // Creating SDK binding for machine-area relationship
//                MachineAreaSetting machineAreaSettings = await _dbContext.MachineAreaSettings.FirstOrDefaultAsync();
//                if (machineAreaSettings.SelectedeFormId == null)
//                {
//                    return new OperationResult(false,
//                        _localizationService.GetString("ErrorCreatingArea", model.Name));
//                }
//
//                Core core = _coreService.GetCore();
//
//                MainElement mainElement = core.TemplateRead((int)machineAreaSettings.SelectedeFormId);
//                List<Site_Dto> sites = core.SiteReadAll(false);
//
//                // Foreach machine-area binding pass sites and get id
//                foreach (var newAreaMachineArea in newArea.MachineAreas)
//                {
//                   newAreaMachineArea.MicrotingeFormSdkId = 
//                        int.Parse(core.CaseCreate(mainElement, "", sites.Select(x => x.SiteId).ToList(), "").FirstOrDefault());
//                }


                await _dbContext.Areas.AddAsync(newArea);
                await _dbContext.SaveChangesAsync();

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

        public async Task<OperationResult> UpdateArea(AreaUpdateModel model)
        {
            try
            {
                //var machineAreaSettings = await _dbContext.MachineAreaSettings
                //    .FirstOrDefaultAsync();
                
                /*if (machineAreaSettings.SelectedeFormId == null)
                {
                    return new OperationResult(false,
                        _localizationService.GetString("ErrorUpdatingArea"));
                }*/
                MachineAreaSetting machineAreaSetting = await _dbContext.MachineAreaSettings.FirstOrDefaultAsync(x => x.Name == MachineAreaSettingsModel.Settings.SdkeFormId.ToString());
                int _sdkeFormId = int.Parse(machineAreaSetting.Value);
                
                var areaForUpdate = await _dbContext.Areas.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (areaForUpdate == null)
                {
                    return new OperationResult(false,
                        _localizationService.GetString("AreaWithIdNotExist", model.Id));
                }

                areaForUpdate.Name = model.Name;
                areaForUpdate.WorkflowState = Constants.WorkflowStates.Processed;
                areaForUpdate.UpdatedByUserId = 2;
                areaForUpdate.UpdatedAt = DateTime.UtcNow;

                var machinesForDelete = await _dbContext.MachineAreas
                    .Where(x => !model.RelatedMachinesIds.Contains(x.MachineId) && x.AreaId == model.Id)
                    .ToListAsync();

                var machineIds = await _dbContext.MachineAreas
                    .Where(x => model.RelatedMachinesIds.Contains(x.MachineId) && x.AreaId == model.Id)
                    .Select(x => x.MachineId)
                    .ToListAsync();

                var core = _coreService.GetCore();
                var mainElement = core.TemplateRead(_sdkeFormId);
                var sites = core.SiteReadAll(false);
                // delete cases
                foreach (var machineForDelete in machinesForDelete)
                {
                    /*core.CaseDelete(
                        (int)machineAreaSettings.SelectedeFormId,
                        machineForDelete.MicrotingeFormSdkId);*/
                    foreach (MachineAreaSite machineAreaSite in machineForDelete.MachineAreaSites)
                    {
                        core.CaseDelete(machineAreaSite.MicrotingEFormSdkId.ToString());
                        machineAreaSite.WorkflowState = Constants.WorkflowStates.Removed;
                    }

                    machineForDelete.WorkflowState = Constants.WorkflowStates.Removed;
                }

                _dbContext.SaveChanges();

                //_dbContext.RemoveRange(machinesForDelete);
                foreach (var machineId in model.RelatedMachinesIds)
                {
                    if (!machineIds.Contains(machineId))
                    {
                        var newAreaMachineArea =
                            new Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea()
                            {
                                AreaId = model.Id,
                                MachineId = machineId
                            };
                        // Add case
                        //var caseId = core.CaseCreate(mainElement, "", sites.Select(x => x.SiteId).ToList(), "")
                        //    .FirstOrDefault();
                        //newAreaMachineArea.MicrotingeFormSdkId = int.Parse(caseId);
                        areaForUpdate.MachineAreas.Add(newAreaMachineArea);
                    }
                }

                _dbContext.SaveChanges();
                foreach (Microting.eFormMachineAreaBase.Infrastructure.Data.Entities.MachineArea machineArea in
                    areaForUpdate.MachineAreas)
                {
                    foreach (Site_Dto siteDto in core.SiteReadAll(false))
                    {
                        //machineArea.MachineAreaSites.Where(x => x.)
                    }

                    
                }
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
                // Obtaining SDK template for machine-area relationship
//                MachineAreaSetting machineAreaSettings = await _dbContext.MachineAreaSettings.FirstOrDefaultAsync();
//                if (machineAreaSettings.SelectedeFormId == null)
//                {
//                    return new OperationResult(false,
//                        _localizationService.GetString("ErrorWhileDeletingArea"));
//                }
//
//                var areaForDelete = await _dbContext.Areas.FirstOrDefaultAsync(x => x.Id == areaId);
//
//                if (areaForDelete == null)
//                {
//                    return new OperationResult(false,
//                        _localizationService.GetString("AreaWithIdNotExist"));
//                }
//
//                var machineAreasForDelete = await _dbContext.MachineAreas
//                    .Where(x => x.AreaId == areaId)
//                    .ToListAsync();
//
//                Core core = _coreService.GetCore();
//
//                // Removing cases
//                foreach (var machineArea in machineAreasForDelete)
//                {
//                    core.CaseDelete((int)machineAreaSettings.SelectedeFormId, machineArea.MicrotingeFormSdkId);
//                }
//
//                _dbContext.Areas.Remove(areaForDelete);
//                await _dbContext.SaveChangesAsync();
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
