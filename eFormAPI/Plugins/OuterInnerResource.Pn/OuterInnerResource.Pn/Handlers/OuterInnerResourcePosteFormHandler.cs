using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eFormCore;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Dto;
using Microting.eForm.Infrastructure.Models;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using OuterInnerResource.Pn.Infrastructure.Helpers;
using OuterInnerResource.Pn.Messages;
using Rebus.Handlers;

namespace OuterInnerResource.Pn.Handlers
{
    public class OuterInnerResourcePosteFormHandler : IHandleMessages<OuterInnerResourcePosteForm>
    {
        private readonly Core _core;
        private readonly OuterInnerResourcePnDbContext _dbContext;

        public OuterInnerResourcePosteFormHandler(Core core, DbContextHelper dbContextHelper)
        {
            _core = core;
            _dbContext = dbContextHelper.GetDbContext();
        }
        
        public async Task Handle(OuterInnerResourcePosteForm message)
        {
            MainElement mainElement = await _core.TemplateRead(message.SdkeFormId);
            OuterInnerResourceSite outerInnerResourceSite =
                await _dbContext.OuterInnerResourceSites.SingleOrDefaultAsync(x =>
                    x.Id == message.OuterInnerResourceSiteId);
            Site_Dto siteDto = await _core.SiteRead(outerInnerResourceSite.MicrotingSdkSiteId);
            
            mainElement.Label = outerInnerResourceSite.OuterInnerResource.InnerResource.Name;
            mainElement.ElementList[0].Label = outerInnerResourceSite.OuterInnerResource.InnerResource.Name;
            mainElement.EndDate = DateTime.Now.AddYears(10).ToUniversalTime();
            mainElement.StartDate = DateTime.Now.ToUniversalTime();
            mainElement.Repeated = 0;
            
            string lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.QuickSyncEnabled.ToString()}"; 

            bool quickSyncEnabled = _dbContext.PluginConfigurationValues.AsNoTracking()
                                        .FirstOrDefault(x => 
                                            x.Name == lookup)?.Value == "true";

            if (quickSyncEnabled)
            {
                mainElement.EnableQuickSync = true;    
            }
            
            List<Folder_Dto> folderDtos = await _core.FolderGetAll(true);
//
            bool folderAlreadyExist = false;
            int _microtingUId = 0;
            foreach (Folder_Dto folderDto in folderDtos)
            {
                if (folderDto.Name == outerInnerResourceSite.OuterInnerResource.OuterResource.Name)
                {
                    folderAlreadyExist = true;
                    _microtingUId = (int)folderDto.MicrotingUId;
                }
            }

            if (!folderAlreadyExist)
            {
                await _core.FolderCreate(outerInnerResourceSite.OuterInnerResource.OuterResource.Name, 
                    "", null);
                folderDtos = await _core.FolderGetAll(true);
            
                foreach (Folder_Dto folderDto in folderDtos)
                {
                    if (folderDto.Name == outerInnerResourceSite.OuterInnerResource.OuterResource.Name)
                    {
                        _microtingUId = (int)folderDto.MicrotingUId;
                    }
                }
            }
            
            mainElement.CheckListFolderName = _microtingUId.ToString();
            
            int? sdkCaseId = await _core.CaseCreate(mainElement, "", siteDto.SiteId);

            outerInnerResourceSite.MicrotingSdkCaseId = sdkCaseId;
            await outerInnerResourceSite.Update(_dbContext);
        }

    }
}