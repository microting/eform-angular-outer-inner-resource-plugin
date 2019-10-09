using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Data.Seed;
using MachineArea.Pn.Infrastructure.Data.Seed.Data;
using MachineArea.Pn.Infrastructure.Models.Settings;
using MachineArea.Pn.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microting.eFormApi.BasePn;
using Microting.eFormApi.BasePn.Infrastructure.Database.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.Application;
using Microting.eFormApi.BasePn.Infrastructure.Settings;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Factories;

namespace MachineArea.Pn
{
    public class EformMachineAreaPlugin : IEformPlugin
    {
        public string Name => "Microting Machine Area plugin";
        public string PluginId => "eform-angular-machinearea-plugin";
        public string PluginPath => PluginAssembly().Location;
        private string _connectionString;
        public string outerResourceName = "Machines";
        public string innerResourceName = "Areas";

        public Assembly PluginAssembly()
        {
            return typeof(EformMachineAreaPlugin).GetTypeInfo().Assembly;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMachineAreaLocalizationService, OuterInnerResourceLocalizationService>();
            services.AddTransient<IAreaService, OuterResourceService>();
            services.AddTransient<IMachineService, InnerResourceService>();
            services.AddTransient<IMachineAreaSettingsService, OuterInnerResourceSettingsService>();
            services.AddTransient<IMachineAreaReportService, OuterInnerResourceReportService>();
            services.AddTransient<IExcelService, ExcelService>();
            services.AddSingleton<IRebusService, RebusService>();
        }

        public void AddPluginConfig(IConfigurationBuilder builder, string connectionString)
        {
            MachineAreaConfigurationSeedData seedData = new MachineAreaConfigurationSeedData();
            OuterInnerResourcePnContextFactory contextFactory = new OuterInnerResourcePnContextFactory();
            builder.AddPluginConfiguration(
                connectionString,
                seedData,
                contextFactory);
        }

        public void ConfigureOptionsServices(
            IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigurePluginDbOptions<MachineAreaBaseSettings>(
                configuration.GetSection("MachineAreaBaseSettings"));
        }

        public void ConfigureDbContext(IServiceCollection services, string connectionString)
        {
            _connectionString = connectionString;
            if (connectionString.ToLower().Contains("convert zero datetime"))
            {
                services.AddDbContext<OuterInnerResourcePnDbContext>(o => o.UseMySql(connectionString,
                    b => b.MigrationsAssembly(PluginAssembly().FullName)));
            }
            else
            {
                services.AddDbContext<OuterInnerResourcePnDbContext>(o => o.UseSqlServer(connectionString,
                    b => b.MigrationsAssembly(PluginAssembly().FullName)));
            }

            OuterInnerResourcePnContextFactory contextFactory = new OuterInnerResourcePnContextFactory();

            using (OuterInnerResourcePnDbContext context = contextFactory.CreateDbContext(new[] {connectionString}))
            {  
                context.Database.Migrate();
                try
                {
                    outerResourceName = context.PluginConfigurationValues.SingleOrDefault(x => x.Name == "MachineAreaBaseSettings:OuterResourceName").Value;
                    innerResourceName = context.PluginConfigurationValues.SingleOrDefault(x => x.Name == "MachineAreaBaseSettings:InnerResourceName").Value;    
                } catch {}
                
            }

            // Seed database
            SeedDatabase(connectionString);
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            IServiceProvider serviceProvider = appBuilder.ApplicationServices;
            IRebusService rebusService = serviceProvider.GetService<IRebusService>();
            rebusService.Start(_connectionString);
        }

        public MenuModel HeaderMenu(IServiceProvider serviceProvider)
        {
            IMachineAreaLocalizationService localizationService = serviceProvider
                .GetService<IMachineAreaLocalizationService>();

            MenuModel result = new MenuModel();
            result.LeftMenu.Add(new MenuItemModel()
            {
                Name = localizationService.GetString("MachineArea"),
                E2EId = "machine-area-pn",
                Link = "",
                MenuItems = new List<MenuItemModel>()
                {
                    new MenuItemModel()
                    {
//                        Name = localizationService.GetString("Machines"),
                        Name = innerResourceName,
                        E2EId = $"machine-area-pn-machines",
                        Link = $"/plugins/machine-area-pn/Machines",
                        Position = 0,
                    },
                    new MenuItemModel()
                    {
//                        Name = localizationService.GetString("Areas"),
                        Name = outerResourceName,
                        E2EId = $"machine-area-pn-areas",
                        Link = $"/plugins/machine-area-pn/Areas",
                        Position = 1,
                    },
                    new MenuItemModel()
                    {
                        Name = localizationService.GetString("Reports"),
                        E2EId = "machine-area-pn-reports",
                        Link = "/plugins/machine-area-pn/reports",
                        Position = 2,
                    },
                    new MenuItemModel()
                    {
                        Name = localizationService.GetString("Settings"),
                        E2EId = "machine-area-pn-settings",
                        Link = "/plugins/machine-area-pn/settings",
                        Position = 3,
                    }
                }
            });
            return result;
        }

        public void SeedDatabase(string connectionString)
        {
            // Get DbContext
            OuterInnerResourcePnContextFactory contextFactory = new OuterInnerResourcePnContextFactory();
            using (OuterInnerResourcePnDbContext context = contextFactory.CreateDbContext(new[] { connectionString }))
            {
                // Seed configuration
                MachineAreaPluginSeed.SeedData(context);
            }
        }
    }
}