/*
The MIT License (MIT)

Copyright (c) 2007 - 2021 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eForm.Infrastructure.Constants;
using Microting.eForm.Infrastructure.Models;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Models.OuterResources;
using OuterInnerResource.Pn.Messages;
using Rebus.Bus;
using Sentry;

namespace OuterInnerResource.Pn.Services;

public class OuterResourceService(
    OuterInnerResourcePnDbContext dbContext,
    IOuterInnerResourceLocalizationService localizationService,
    ILogger<OuterResourceService> logger,
    IRebusService rebusService,
    IEFormCoreService coreHelper)
    : IOuterResourceService
{
    private readonly IBus _bus = rebusService.GetBus();

    public async Task<OperationDataResult<OuterResourcesModel>> Index(OuterResourceRequestModel requestModel)
    {
        try
        {
            var outerResourcesModel = new OuterResourcesModel();

            var query = dbContext.OuterResources.AsQueryable();
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
                query = dbContext.OuterResources
                    .OrderBy(x => x.Id);
            }

            query = query.Where(x => x.WorkflowState != Constants.WorkflowStates.Removed);

            if (requestModel.PageSize != null)
            {
                query = query
                    .Skip(requestModel.Offset)
                    .Take((int)requestModel.PageSize);
            }

            var outerResourceList = await query.Select(x => new OuterResourceModel()
            {
                Name = x.Name,
                Id = x.Id,
                ExternalId = x.ExternalId,
                RelatedInnerResourcesIds = dbContext.OuterInnerResources.AsNoTracking().Where(y =>
                        y.OuterResourceId == x.Id && y.WorkflowState != Constants.WorkflowStates.Removed)
                    .Select(z => z.InnerResourceId).ToList()
            }).AsNoTracking().ToListAsync();

            outerResourcesModel.Total = await dbContext.OuterResources.AsNoTracking()
                .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed).CountAsync();
            outerResourcesModel.OuterResourceList = outerResourceList;

            try
            {
                outerResourcesModel.Name = dbContext.PluginConfigurationValues.FirstOrDefault(x =>
                        x.Name == "OuterInnerResourceSettings:OuterResourceName")
                    ?.Value;
            }
            catch (Exception)
            {
                // ignored
            }

            return new OperationDataResult<OuterResourcesModel>(true, outerResourcesModel);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationDataResult<OuterResourcesModel>(false,
                localizationService.GetString("ErrorObtainOuterResources"));
        }
    }

    public async Task<OperationDataResult<OuterResourceModel>> Get(int id)
    {
        try
        {
            var outerResource = await dbContext.OuterResources.Select(x => new OuterResourceModel()
                {
                    Name = x.Name,
                    Id = x.Id,
                    ExternalId = x.ExternalId
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            if (outerResource == null)
            {
                return new OperationDataResult<OuterResourceModel>(false,
                    localizationService.GetString("OuterResourceWithIdNotExist", id));
            }

            var outerInnerResources = await dbContext.OuterInnerResources
                .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed &&
                            x.OuterResourceId == outerResource.Id).AsNoTracking().ToListAsync();

            outerResource.RelatedInnerResourcesIds = new List<int>();
            foreach (var outerInnerResource in outerInnerResources)
            {
                outerResource.RelatedInnerResourcesIds.Add(outerInnerResource.InnerResourceId);
            }

            return new OperationDataResult<OuterResourceModel>(true, outerResource);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationDataResult<OuterResourceModel>(false,
                localizationService.GetString("ErrorObtainOuterResource"));
        }
    }

    public async Task<OperationResult> Create(OuterResourceModel model)
    {
        try
        {
            var outerResource = new OuterResource()
            {
                Name = model.Name,
                ExternalId = model.ExternalId
            };

            await outerResource.Create(dbContext);
            model.Id = outerResource.Id;

            if (model.RelatedInnerResourcesIds != null)
            {
                foreach (var innerResourceId in model.RelatedInnerResourcesIds)
                {
                    var macth = await dbContext.OuterInnerResources.FirstOrDefaultAsync(x =>
                        x.OuterResourceId == model.Id
                        && x.InnerResourceId == innerResourceId);
                    if (macth == null)
                    {
                        var
                            outerInnerResource =
                                new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.
                                    OuterInnerResource
                                    {
                                        InnerResourceId = innerResourceId,
                                        OuterResourceId = model.Id
                                    };
                        await outerInnerResource.Create(dbContext);
                    }
                }

                await _bus.SendLocal(new OuterInnerResourceCreate(null, model));
            }

            return new OperationResult(true,
                localizationService.GetString("OuterResourceCreatedSuccessfully", model.Name));
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationResult(false,
                localizationService.GetString("ErrorCreatingOuterResource"));
        }
    }

    public async Task<OperationResult> Update(OuterResourceModel model)
    {
        int? oldExternalId;
        try
        {
            var outerResource =
                await dbContext.OuterResources.FirstAsync(x => x.Id == model.Id);

            var core = await coreHelper.GetCore();
            var sdkDbContext = core.DbContextHelper.GetDbContext();

            var folderQuery = sdkDbContext.Folders
                .AsNoTracking()
                .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                .Where(x => x.FolderTranslations.Any(y =>
                    y.Name == outerResource.Name))
                .Select(x => new { x.MicrotingUid, x.Id, x.Name, x.ParentId });

            if (folderQuery.First().Name != model.Name)
            {
                List<CommonTranslationsModel> translationsModels = new List<CommonTranslationsModel>();
                var translations = new CommonTranslationsModel()
                {
                    Description = "",
                    LanguageId = 1,
                    Name = model.Name
                };
                translationsModels.Add(translations);
                await core.FolderUpdate(folderQuery.First().Id, translationsModels, folderQuery.First().ParentId);
            }

            oldExternalId = outerResource.ExternalId;
            outerResource.ExternalId = model.ExternalId;
            outerResource.Name = model.Name;
            await outerResource.Update(dbContext);

            var
                outerInnerResources =
                    await dbContext.OuterInnerResources.Where(x =>
                        x.OuterResourceId == outerResource.Id
                        && x.WorkflowState != Constants.WorkflowStates.Removed).ToListAsync();

            var requestedInnerResourceIds = model.RelatedInnerResourcesIds;
            var deployedInnerResourceIds = new List<int>();
            var toBeDeployed = new List<int>();

            foreach (var outerInnerResource in outerInnerResources)
            {
                deployedInnerResourceIds.Add(outerInnerResource.InnerResourceId);

                if (!model.RelatedInnerResourcesIds.Contains(outerInnerResource.InnerResourceId))
                {
                    await outerInnerResource.Delete(dbContext);
                    await _bus.SendLocal(new OuterInnerResourceUpdate(outerInnerResource.Id, null, null, null, null,
                        null));
                }
            }

            if (requestedInnerResourceIds.Count != 0)
            {
                toBeDeployed.AddRange(requestedInnerResourceIds.Where(x =>
                    !deployedInnerResourceIds.Contains(x)));
            }

            foreach (var innerResourceId in toBeDeployed)
            {
                var innerResource = dbContext.InnerResources.FirstOrDefault(x =>
                    x.Id == innerResourceId);
                if (innerResource != null)
                {
                    var
                        outerInnerResource = await dbContext.OuterInnerResources.FirstOrDefaultAsync(x =>
                            x.OuterResourceId == outerResource.Id
                            && x.InnerResourceId == innerResourceId);

                    if (outerInnerResource == null)
                    {
                        outerInnerResource =
                            new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.
                                OuterInnerResource()
                                {
                                    InnerResourceId = innerResourceId,
                                    OuterResourceId = outerResource.Id
                                };
                        await outerInnerResource.Create(dbContext);
                    }
                    else
                    {
                        outerInnerResource.WorkflowState = Constants.WorkflowStates.Created;
                        await outerInnerResource.Update(dbContext);
                    }

                    await _bus.SendLocal(new OuterInnerResourceUpdate(outerInnerResource.Id, null, oldExternalId,
                        null, null, null));
                }
            }

            return new OperationResult(true, localizationService.GetString("OuterResourceUpdatedSuccessfully"));
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationResult(false,
                localizationService.GetString("ErrorUpdatingOuterResource"));
        }
    }

    public async Task<OperationResult> Delete(int outerResourceId)
    {
        try
        {
            var outerResource = await dbContext.OuterResources.FirstAsync(x => x.Id == outerResourceId);

            await outerResource.Delete(dbContext);
            await _bus.SendLocal(new OuterInnerResourceDelete(null,
                new OuterResourceModel() { Id = outerResourceId }));
            return new OperationResult(true, localizationService.GetString("OuterResourceDeletedSuccessfully"));
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationResult(false,
                localizationService.GetString("ErrorWhileDeletingOuterResource"));
        }
    }
}