using System;
using System.Collections.Generic;
using System.Reflection;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microting.eFormApi.BasePn;
using Microting.eFormApi.BasePn.Infrastructure.Models.Application;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Factories;

namespace MachineArea.Pn
{
    public class EformMachineAreaPlugin : IEformPlugin
    {
        public string Name => "Microting Machine Area plugin";
        public string PluginId => "eform-angular-machinearea-plugin";
        public string PluginPath => PluginAssembly().Location;
        private string _connectionString;

        public Assembly PluginAssembly()
        {
            return typeof(EformMachineAreaPlugin).GetTypeInfo().Assembly;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMachineAreaLocalizationService, MachineAreaLocalizationService>();
            services.AddTransient<IAreaService, AreaService>();
            services.AddTransient<IMachineService, MachineService>();
            services.AddTransient<IMachineAreaSettingsService, MachineAreaSettingsService>();
            services.AddTransient<IMachineAreaReportService, MachineAreaReportService>();
            services.AddTransient<IExcelService, ExcelService>();
            services.AddSingleton<IRebusService, RebusService>();
        }

        public void ConfigureDbContext(IServiceCollection services, string connectionString)
        {
            _connectionString = connectionString;
            if (connectionString.ToLower().Contains("convert zero datetime"))
            {
                services.AddDbContext<MachineAreaPnDbContext>(o => o.UseMySql(connectionString,
                    b => b.MigrationsAssembly(PluginAssembly().FullName)));
            }
            else
            {                
                services.AddDbContext<MachineAreaPnDbContext>(o => o.UseSqlServer(connectionString,
                    b => b.MigrationsAssembly(PluginAssembly().FullName)));
            }

            MachineAreaPnContextFactory contextFactory = new MachineAreaPnContextFactory();
            var context = contextFactory.CreateDbContext(new[] {connectionString});
            context.Database.Migrate();

            // Seed database
            SeedDatabase(connectionString);
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            var serviceProvider = appBuilder.ApplicationServices;
            IRebusService rebusService = serviceProvider.GetService<IRebusService>();
            rebusService.Start(_connectionString);

        }

        public MenuModel HeaderMenu(IServiceProvider serviceProvider)
        {
            var localizationService = serviceProvider
                .GetService<IMachineAreaLocalizationService>();

            var result = new MenuModel();
            result.LeftMenu.Add(new MenuItemModel()
            {
                
                Name = localizationService.GetString("MachineArea"),
                E2EId = "",
                Link = "",
                MenuItems = new List<MenuItemModel>()
                {
                    new MenuItemModel()
                    {
                        Name =  localizationService.GetString("Machines"), 
                        E2EId = "machine-area-pn-machines",
                        Link = "/plugins/machine-area-pn/machines",
                        Position = 0,
                    },
                    new MenuItemModel()
                    {
                        Name =  localizationService.GetString("Areas"),
                        E2EId = "machine-area-pn-areas",
                        Link = "/plugins/machine-area-pn/areas",
                        Position = 1,
                    },
                    new MenuItemModel()
                    {
                        Name =  localizationService.GetString("Reports"),
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
           
        }
    }
}