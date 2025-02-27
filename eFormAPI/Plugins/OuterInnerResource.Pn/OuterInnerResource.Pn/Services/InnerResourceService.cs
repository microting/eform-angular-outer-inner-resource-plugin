﻿/*
The MIT License (MIT)

Copyright (c) 2007 - 2019 Microting A/S

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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eForm.Infrastructure.Constants;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Models.InnerResources;
using OuterInnerResource.Pn.Messages;
using Rebus.Bus;
using Sentry;

namespace OuterInnerResource.Pn.Services;

using Microting.eFormApi.BasePn.Infrastructure.Helpers;

public class InnerResourceService(
    OuterInnerResourcePnDbContext dbContext,
    IOuterInnerResourceLocalizationService localizationService,
    ILogger<InnerResourceService> logger,
    IRebusService rebusService)
    : IInnerResourceService
{
    private readonly IBus _bus = rebusService.GetBus();

    public async Task<OperationDataResult<InnerResourcesModel>> Index(InnerResourceRequestModel requestModel)
    {
        try
        {
            var innerResourcesModel = new InnerResourcesModel();

            var query = dbContext.InnerResources
                .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                .AsNoTracking()
                .AsQueryable();

            query = QueryHelper.AddSortToQuery(query, requestModel.Sort, requestModel.IsSortDsc);

            innerResourcesModel.Total = await query.Select(x => x.Id).CountAsync();

            query = query
                .Skip(requestModel.Offset)
                .Take(requestModel.PageSize);

            innerResourcesModel.InnerResourceList = await query
                .Select(x => new InnerResourceModel
                {
                    Name = x.Name,
                    Id = x.Id,
                    ExternalId = x.ExternalId,
                    RelatedOuterResourcesIds = dbContext.OuterInnerResources.Where(y =>
                        y.InnerResourceId == x.Id && y.WorkflowState != Constants.WorkflowStates.Removed).Select(z => z.OuterResourceId).ToList()
                }).ToListAsync();

            innerResourcesModel.Name = dbContext.PluginConfigurationValues
                .FirstOrDefault(x => x.Name == "OuterInnerResourceSettings:InnerResourceName")
                ?.Value;

            return new OperationDataResult<InnerResourcesModel>(true, innerResourcesModel);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationDataResult<InnerResourcesModel>(false,
                localizationService.GetString("ErrorObtainInnerResources"));
        }
    }

    public async Task<OperationDataResult<InnerResourceModel>> Get(int innerResourceId)
    {
        try
        {
            var innerResource = await dbContext.InnerResources.Select(x => new InnerResourceModel()
                {
                    Name = x.Name,
                    Id = x.Id,
                    ExternalId = x.ExternalId
                })
                .FirstOrDefaultAsync(x => x.Id == innerResourceId);

            if (innerResource == null)
            {
                return new OperationDataResult<InnerResourceModel>(false,
                    localizationService.GetString("InnerResourceWithIdNotExist"));
            }

            var outerInnerResources = await dbContext.OuterInnerResources
                .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed && x.InnerResourceId == innerResource.Id).AsNoTracking().ToListAsync();

            innerResource.RelatedOuterResourcesIds = new List<int>();
            foreach (var outerInnerResource in outerInnerResources)
            {
                innerResource.RelatedOuterResourcesIds.Add(outerInnerResource.OuterResourceId);
            }

            return new OperationDataResult<InnerResourceModel>(true, innerResource);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationDataResult<InnerResourceModel>(false,
                localizationService.GetString("ErrorObtainInnerResource"));
        }
    }

    public async Task<OperationResult> Create(InnerResourceModel model)
    {
        try
        {
            var innerResource = new InnerResource()
            {
                Name = model.Name,
                ExternalId = model.ExternalId,
            };

            await innerResource.Create(dbContext);
            model.Id = innerResource.Id;

            if (model.RelatedOuterResourcesIds != null)
            {
                foreach (var outerResourceId in model.RelatedOuterResourcesIds)
                {
                    var macth = await dbContext.OuterInnerResources.FirstOrDefaultAsync(x =>
                        x.InnerResourceId == model.Id
                        && x.OuterResourceId == outerResourceId);
                    if (macth == null)
                    {
                        var
                            outerInnerResource =
                                new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.
                                    OuterInnerResource
                                    {
                                        OuterResourceId = outerResourceId,
                                        InnerResourceId = model.Id
                                    };
                        await outerInnerResource.Create(dbContext);
                    }
                }
                await _bus.SendLocal(new OuterInnerResourceCreate(model, null));
            }

            return new OperationResult(true,
                localizationService.GetString("InnerResourceCreatedSuccessfully", model.Name));
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationResult(false,
                localizationService.GetString("ErrorCreatingInnerResource"));
        }
    }

    public async Task<OperationResult> Update(InnerResourceModel model)
    {
        var oldName = "";
        int? oldExternalId;
        try
        {
            var innerResource =
                await dbContext.InnerResources.FirstAsync(x => x.Id == model.Id);

            oldName = innerResource.Name;
            oldExternalId = innerResource.ExternalId;
            innerResource.ExternalId = model.ExternalId;
            innerResource.Name = model.Name;
            await innerResource.Update(dbContext);

            var
                outerInnerResources =
                    await dbContext.OuterInnerResources.Where(x =>
                        x.InnerResourceId == innerResource.Id
                        && x.WorkflowState != Constants.WorkflowStates.Removed).ToListAsync();

            var requestedOuterResourceIds = model.RelatedOuterResourcesIds;
            var deployedOuterResourceIds = new List<int>();
            var toBeDeployed = new List<int>();

            foreach (var outerInnerResource in outerInnerResources)
            {
                deployedOuterResourceIds.Add(outerInnerResource.OuterResourceId);

                if (!model.RelatedOuterResourcesIds.Contains(outerInnerResource.OuterResourceId))
                {
                    await outerInnerResource.Delete(dbContext);
                    await _bus.SendLocal(new OuterInnerResourceUpdate(outerInnerResource.Id, oldName, null, null, null, innerResource.Name));
                }
            }

            if (requestedOuterResourceIds.Count != 0)
            {
                toBeDeployed.AddRange(requestedOuterResourceIds.Where(x =>
                    !deployedOuterResourceIds.Contains(x)));
            }

            if (toBeDeployed.Count == 0)
            {
                var
                    outerInnerResourceList =
                        await dbContext.OuterInnerResources.Where(x =>
                            x.InnerResourceId == innerResource.Id
                            && x.WorkflowState != Constants.WorkflowStates.Removed).ToListAsync();
                foreach (var outerInnerResource in outerInnerResourceList)
                {
                    await _bus.SendLocal(new OuterInnerResourceUpdate(outerInnerResource.Id, oldName,
                        oldExternalId, null, null, innerResource.Name));
                }

            } else {
                foreach (var outerResourceId in toBeDeployed)
                {
                    var outerResource = dbContext.OuterResources.FirstOrDefault(x =>
                        x.Id == outerResourceId);
                    if (outerResource != null)
                    {
                        var
                            outerInnerResource = await dbContext.OuterInnerResources.FirstOrDefaultAsync(x =>
                                x.InnerResourceId == innerResource.Id
                                && x.OuterResourceId == outerResourceId);

                        if (outerInnerResource == null)
                        {
                            outerInnerResource =
                                new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.
                                    OuterInnerResource()
                                    {
                                        OuterResourceId = outerResourceId,
                                        InnerResourceId = innerResource.Id
                                    };
                            await outerInnerResource.Create(dbContext);
                        }
                        else
                        {
                            outerInnerResource.WorkflowState = Constants.WorkflowStates.Created;
                            await outerInnerResource.Update(dbContext);
                        }

                        await _bus.SendLocal(new OuterInnerResourceUpdate(outerInnerResource.Id, oldName,
                            oldExternalId, null, null, innerResource.Name));
                    }
                }
            }
            return new OperationResult(true, localizationService.GetString("InnerResourceUpdatedSuccessfully"));
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationResult(false,
                localizationService.GetString("ErrorUpdatingInnerResource"));
        }
    }

    public async Task<OperationResult> Delete(int innerResourceId)
    {
        try
        {
            var innerResource = await dbContext.InnerResources.FirstAsync(x => x.Id == innerResourceId);

            await innerResource.Delete(dbContext);
            await _bus.SendLocal(new OuterInnerResourceDelete(new InnerResourceModel() { Id = innerResourceId }, null));
            return new OperationResult(true, localizationService.GetString("InnerResourceDeletedSuccessfully"));
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            logger.LogError(e.Message);
            logger.LogTrace(e.StackTrace);
            return new OperationResult(false,
                localizationService.GetString("ErrorWhileDeletingInnerResource"));
        }
    }
}