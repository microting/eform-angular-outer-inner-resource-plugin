/*
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

using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Sentry;

namespace OuterInnerResource.Pn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microting.eFormApi.BasePn;
    using Microting.eFormApi.BasePn.Infrastructure.Consts;
    using Microting.eFormApi.BasePn.Infrastructure.Database.Extensions;
    using Microting.eFormApi.BasePn.Infrastructure.Helpers;
    using Microting.eFormApi.BasePn.Infrastructure.Models.Application;
    using Microting.eFormApi.BasePn.Infrastructure.Models.Application.NavigationMenu;
    using Microting.eFormApi.BasePn.Infrastructure.Settings;
    using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
    using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;
    using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Factories;
    using Abstractions;
    using Infrastructure.Data.Seed;
    using Infrastructure.Data.Seed.Data;
    using Infrastructure.Models.Settings;
    using Services;

    public class EformOuterInnerResourcePlugin : IEformPlugin
    {
        public string Name => "Microting Outer Inner Resource plugin";
        public string PluginId => "eform-angular-outer-inner-resource-plugin";
        public string PluginPath => PluginAssembly().Location;
        public string PluginBaseUrl => "outer-inner-resource-pn";

        private string _connectionString;
        private string _outerResourceName = "OuterResources";
        private string _innerResourceName = "InnerResources";
        private int _maxParallelism = 1;
        private int _numberOfWorkers = 1;

        public Assembly PluginAssembly()
        {
            return typeof(EformOuterInnerResourcePlugin).GetTypeInfo().Assembly;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IOuterInnerResourceLocalizationService, OuterInnerResourceLocalizationService>();
            services.AddTransient<IOuterResourceService, OuterResourceService>();
            services.AddTransient<IInnerResourceService, InnerResourceService>();
            services.AddTransient<IOuterInnerResourceSettingsService, OuterInnerResourceSettingsService>();
            services.AddTransient<IOuterInnerResourceReportService, OuterInnerResourceReportService>();
            services.AddTransient<IResourceTimeRegistrationService, ResourceTimeRegistrationService>();
            services.AddTransient<IExcelService, ExcelService>();
            services.AddSingleton<IRebusService, RebusService>();
            services.AddControllers();
        }

        public void AddPluginConfig(IConfigurationBuilder builder, string connectionString)
        {
            var seedData = new OuterInnerResourceConfigurationSeedData();
            var contextFactory = new OuterInnerResourcePnContextFactory();
            builder.AddPluginConfiguration(
                connectionString,
                seedData,
                contextFactory);
        }

        public void ConfigureOptionsServices(
            IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigurePluginDbOptions<OuterInnerResourceSettings>(
                configuration.GetSection("OuterInnerResourceSettings"));
        }

        public void ConfigureDbContext(IServiceCollection services, string connectionString)
        {
            SentrySdk.Init(options =>
            {
                // A Sentry Data Source Name (DSN) is required.
                // See https://docs.sentry.io/product/sentry-basics/dsn-explainer/
                // You can set it in the SENTRY_DSN environment variable, or you can set it in code here.
                options.Dsn = "https://9bf9ba91ddb235518478803b2b202fc8@o4506241219428352.ingest.us.sentry.io/4508266875781120";

                // When debug is enabled, the Sentry client will emit detailed debugging information to the console.
                // This might be helpful, or might interfere with the normal operation of your application.
                // We enable it here for demonstration purposes when first trying Sentry.
                // You shouldn't do this in your applications unless you're troubleshooting issues with Sentry.
                options.Debug = false;

                // This option is recommended. It enables Sentry's "Release Health" feature.
                options.AutoSessionTracking = true;

                // This option is recommended for client applications only. It ensures all threads use the same global scope.
                // If you're writing a background service of any kind, you should remove this.
                options.IsGlobalModeEnabled = true;

                // This option will enable Sentry's tracing features. You still need to start transactions and spans.
                //options.EnableTracing = true;
            });

            string pattern = @"Database=(\d+)_eform-angular-outer-inner-resource-plugin;";
            Match match = Regex.Match(connectionString!, pattern);

            if (match.Success)
            {
                string numberString = match.Groups[1].Value;
                int number = int.Parse(numberString);
                SentrySdk.ConfigureScope(scope =>
                {
                    scope.SetTag("customerNo", number.ToString());
                    Console.WriteLine("customerNo: " + number);
                    scope.SetTag("osVersion", Environment.OSVersion.ToString());
                    Console.WriteLine("osVersion: " + Environment.OSVersion);
                    scope.SetTag("osArchitecture", RuntimeInformation.OSArchitecture.ToString());
                    Console.WriteLine("osArchitecture: " + RuntimeInformation.OSArchitecture);
                    scope.SetTag("osName", RuntimeInformation.OSDescription);
                    Console.WriteLine("osName: " + RuntimeInformation.OSDescription);
                });
            }
            _connectionString = connectionString;
            services.AddDbContext<OuterInnerResourcePnDbContext>(o =>
                o.UseMySql(connectionString, new MariaDbServerVersion(
                    new Version(10, 4, 0)), mySqlOptionsAction: builder =>
                {
                    builder.EnableRetryOnFailure();
                    builder.MigrationsAssembly(PluginAssembly().FullName);
                    builder.TranslateParameterizedCollectionsToConstants();
                }));

            var contextFactory = new OuterInnerResourcePnContextFactory();

            using (var context = contextFactory.CreateDbContext(new[] { connectionString }))
            {
                context.Database.Migrate();
                try
                {
                    _outerResourceName = context.PluginConfigurationValues.FirstOrDefault(x => x.Name == "OuterInnerResourceSettings:OuterResourceName")?.Value;
                    _innerResourceName = context.PluginConfigurationValues.FirstOrDefault(x => x.Name == "OuterInnerResourceSettings:InnerResourceName")?.Value;
                    var temp = context.PluginConfigurationValues
                        .FirstOrDefault(x => x.Name == "OuterInnerResourceSettings:MaxParallelism")?.Value;
                    _maxParallelism = string.IsNullOrEmpty(temp) ? 1 : int.Parse(temp);

                    temp = context.PluginConfigurationValues
                        .FirstOrDefault(x => x.Name == "OuterInnerResourceSettings:NumberOfWorkers")?.Value;
                    _numberOfWorkers = string.IsNullOrEmpty(temp) ? 1 : int.Parse(temp);
                }
                catch
                {
                    // ignored
                }
            }

            // Seed database
            SeedDatabase(connectionString);


        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            var serviceProvider = appBuilder.ApplicationServices;

            IRebusService rebusService = serviceProvider.GetService<IRebusService>();

            WindsorContainer container = rebusService.GetContainer();
            container.Register(Component.For<EformOuterInnerResourcePlugin>().Instance(this));
            rebusService.Start(_connectionString).GetAwaiter().GetResult();
        }

        public List<PluginMenuItemModel> GetNavigationMenu(IServiceProvider serviceProvider)
        {
            var pluginMenu = new List<PluginMenuItemModel>
            {
                new()
                {
                        Name = "Dropdown",
                        E2EId = "outer-inner-resource-pn",
                        Link = "",
                        Type = MenuItemTypeEnum.Dropdown,
                        Position = 0,
                        Translations = new List<PluginMenuTranslationModel>
                        {
                            new()
                            {
                                 LocaleName = LocaleNames.English,
                                 Name = "Outer/Inner resources",
                                 Language = LanguageNames.English,
                            },
                            new()
                            {
                                 LocaleName = LocaleNames.German,
                                 Name = "Maschinenbereich",
                                 Language = LanguageNames.German,
                            },
                            new()
                            {
                                 LocaleName = LocaleNames.Danish,
                                 Name = "Ydre/Indre resourcer",
                                 Language = LanguageNames.Danish,
                            }
                        },
                        ChildItems = new List<PluginMenuItemModel>
                        {
                            new()
                            {
                                Name = _innerResourceName,
                                E2EId = "outer-inner-resource-pn-inner-resources",
                                Link = "/plugins/outer-inner-resource-pn/inner-resources",
                                Type = MenuItemTypeEnum.Link,
                                Position = 0,
                                MenuTemplate = new PluginMenuTemplateModel
                                {
                                    Name = _innerResourceName,
                                    E2EId = "outer-inner-resource-pn-inner-resources",
                                    DefaultLink = "/plugins/outer-inner-resource-pn/inner-resources",
                                    Permissions = new List<PluginMenuTemplatePermissionModel>(),
                                    Translations = new List<PluginMenuTranslationModel>
                                    {
                                        new()
                                        {
                                            LocaleName = LocaleNames.English,
                                            Name = "Inner resources",
                                            Language = LanguageNames.English,
                                        },
                                        new()
                                        {
                                            LocaleName = LocaleNames.German,
                                            Name = "Interne ressourcen",
                                            Language = LanguageNames.German,
                                        },
                                        new()
                                        {
                                            LocaleName = LocaleNames.Danish,
                                            Name = "Indre resourcer",
                                            Language = LanguageNames.Danish,
                                        },
                                    }
                                },
                                Translations = new List<PluginMenuTranslationModel>
                                    {
                                        new()
                                        {
                                            LocaleName = LocaleNames.English,
                                            Name = "Inner resources",
                                            Language = LanguageNames.English,
                                        },
                                        new()
                                        {
                                            LocaleName = LocaleNames.German,
                                            Name = "Interne ressourcen",
                                            Language = LanguageNames.German,
                                        },
                                        new()
                                        {
                                            LocaleName = LocaleNames.Danish,
                                            Name = "Indre resourcer",
                                            Language = LanguageNames.Danish,
                                        },
                                    }
                            },
                            new()
                            {
                                Name = _outerResourceName,
                                E2EId = "outer-inner-resource-pn-outer-resources",
                                Link = "/plugins/outer-inner-resource-pn/outer-resources",
                                Type = MenuItemTypeEnum.Link,
                                Position = 1,
                                MenuTemplate = new PluginMenuTemplateModel
                                {
                                    Name = _outerResourceName,
                                    E2EId = "outer-inner-resource-pn-outer-resources",
                                    DefaultLink = "/plugins/outer-inner-resource-pn/outer-resources",
                                    Permissions = new List<PluginMenuTemplatePermissionModel>(),
                                    Translations = new List<PluginMenuTranslationModel>
                                    {
                                        new()
                                        {
                                            LocaleName = LocaleNames.English,
                                            Name = "Outer resources",
                                            Language = LanguageNames.English,
                                        },
                                        new()
                                        {
                                            LocaleName = LocaleNames.German,
                                            Name = "Externe ressourcen",
                                            Language = LanguageNames.German,
                                        },
                                        new()
                                        {
                                            LocaleName = LocaleNames.Danish,
                                            Name = "Ydre resourcer",
                                            Language = LanguageNames.Danish,
                                        },
                                    }
                                },
                                Translations = new List<PluginMenuTranslationModel>
                                    {
                                        new()
                                        {
                                            LocaleName = LocaleNames.English,
                                            Name = "Outer resources",
                                            Language = LanguageNames.English,
                                        },
                                        new()
                                        {
                                            LocaleName = LocaleNames.German,
                                            Name = "Externe ressourcen",
                                            Language = LanguageNames.German,
                                        },
                                        new()
                                        {
                                            LocaleName = LocaleNames.Danish,
                                            Name = "Ydre resourcer",
                                            Language = LanguageNames.Danish,
                                        },
                                    }
                            },
                            new()
                            {
                                Name = "Reports",
                                E2EId = "outer-inner-resource-pn-reports",
                                Link = "/plugins/outer-inner-resource-pn/reports",
                                Type = MenuItemTypeEnum.Link,
                                Position = 2,
                                MenuTemplate = new PluginMenuTemplateModel
                                {
                                    Name = "Reports",
                                    E2EId = "outer-inner-resource-pn-reports",
                                    DefaultLink = "/plugins/outer-inner-resource-pn/reports",
                                    Permissions = new List<PluginMenuTemplatePermissionModel>(),
                                    Translations = new List<PluginMenuTranslationModel>
                                    {
                                        new()
                                        {
                                            LocaleName = LocaleNames.English,
                                            Name = "Reports",
                                            Language = LanguageNames.English,
                                        },
                                        new()
                                        {
                                            LocaleName = LocaleNames.German,
                                            Name = "Berichte",
                                            Language = LanguageNames.German,
                                        },
                                        new()
                                        {
                                            LocaleName = LocaleNames.Danish,
                                            Name = "Rapport",
                                            Language = LanguageNames.Danish,
                                        },
                                    }
                                },
                                Translations = new List<PluginMenuTranslationModel>
                                    {
                                        new()
                                        {
                                            LocaleName = LocaleNames.English,
                                            Name = "Reports",
                                            Language = LanguageNames.English,
                                        },
                                        new()
                                        {
                                            LocaleName = LocaleNames.German,
                                            Name = "Berichte",
                                            Language = LanguageNames.German,
                                        },
                                        new()
                                        {
                                            LocaleName = LocaleNames.Danish,
                                            Name = "Rapport",
                                            Language = LanguageNames.Danish,
                                        },
                                    }
                            }
                        }
                    }
            };

            return pluginMenu;
        }

        public MenuModel HeaderMenu(IServiceProvider serviceProvider)
        {
            var localizationService = serviceProvider
                .GetService<IOuterInnerResourceLocalizationService>();

            var result = new MenuModel();
            result.LeftMenu.Add(new MenuItemModel
            {
                Name = localizationService.GetString("OuterInnerResource"),
                E2EId = "outer-inner-resource-pn",
                Link = "",
                Guards = new List<string> { OuterInnerResourceClaims.AccessOuterInnerResourcePlugin },
                MenuItems = new List<MenuItemModel>
                {
                    new()
                    {
                        // Name = localizationService.GetString("Machines"),
                        Name = _innerResourceName,
                        E2EId = "outer-inner-resource-pn-inner-resources",
                        Link = "/plugins/outer-inner-resource-pn/inner-resources",
                        Position = 0,
                    },
                    new()
                    {
                        // Name = localizationService.GetString("Areas"),
                        Name = _outerResourceName,
                        E2EId = "outer-inner-resource-pn-outer-resources",
                        Link = "/plugins/outer-inner-resource-pn/outer-resources",
                        Position = 1,
                    },
                    new()
                    {
                        Name = localizationService.GetString("Reports"),
                        E2EId = "outer-inner-resource-pn-reports",
                        Link = "/plugins/outer-inner-resource-pn/reports",
                        Position = 2,
                    }
                }
            });
            return result;
        }

        public void SeedDatabase(string connectionString)
        {
            // Get DbContext
            var contextFactory = new OuterInnerResourcePnContextFactory();
            using var context = contextFactory.CreateDbContext(new[] { connectionString });
            // Seed configuration
            OuterInnerResourcePluginSeed.SeedData(context);
        }

        public PluginPermissionsManager GetPermissionsManager(string connectionString)
        {
            var contextFactory = new OuterInnerResourcePnContextFactory();
            var context = contextFactory.CreateDbContext(new[] { connectionString });
            return new PluginPermissionsManager(context);
        }
    }
}
