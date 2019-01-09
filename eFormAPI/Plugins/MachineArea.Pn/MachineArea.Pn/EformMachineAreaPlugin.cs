using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Data;
using MachineArea.Pn.Infrastructure.Data.Factories;
using MachineArea.Pn.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microting.eFormApi.BasePn;
using Microting.eFormApi.BasePn.Infrastructure.Models.Application;

namespace MachineArea.Pn
{
    public class EformMachineAreaPlugin : IEformPlugin
    {
        public string Name => "Microting MachineArea plugin";
        public string PluginId => "EFormMachineAreaPn";
        public string PluginPath => PluginAssembly().Location;

        public Assembly PluginAssembly()
        {
            return typeof(EformMachineAreaPlugin).GetTypeInfo().Assembly;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMachineAreaLocalizationService, MachineAreaLocalizationService>();
            services.AddTransient<IAreaService, AreaService>();
            services.AddTransient<IMachineService, MachineService>();
        }

        public void ConfigureDbContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<MachineAreaPnDbContext>(o => o.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(PluginAssembly().FullName)));

            MachineAreaPnContextFactory contextFactory = new MachineAreaPnContextFactory();
            using (MachineAreaPnDbContext context = contextFactory.CreateDbContext(new[] { connectionString }))
            {
                context.Database.Migrate();
            }

            // Seed database
            SeedDatabase(connectionString);
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
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