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

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using eFormCore;
using Microting.eForm.Dto;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Factories;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Helpers;
using OuterInnerResource.Pn.Installers;
using Rebus.Bus;

namespace OuterInnerResource.Pn.Services
{
    public class RebusService : IRebusService
    {
        private IBus _bus;
        private IWindsorContainer _container;
        private string _connectionString;
        private readonly IEFormCoreService _coreHelper;
        private DbContextHelper _dbContextHelper;


        public RebusService(IEFormCoreService coreHelper)
        {
            //_dbContext = dbContext;
            _coreHelper = coreHelper;
            _container = new WindsorContainer();
        }

        public async Task Start(string connectionString)
        {
            _connectionString = connectionString;

            Core core = await _coreHelper.GetCore();
            _connectionString = connectionString;
            var dbPrefix = Regex.Match(_connectionString, @"Database=(\d*)_").Groups[1].Value;
            var rabbitmqHost = core.GetSdkSetting(Settings.rabbitMqHost).GetAwaiter().GetResult();
            Console.WriteLine($"rabbitmqHost: {rabbitmqHost}");
            var rabbitMqUser = core.GetSdkSetting(Settings.rabbitMqUser).GetAwaiter().GetResult();
            Console.WriteLine($"rabbitMqUser: {rabbitMqUser}");
            var rabbitMqPassword = core.GetSdkSetting(Settings.rabbitMqPassword).GetAwaiter().GetResult();
            Console.WriteLine($"rabbitMqPassword: {rabbitMqPassword}");
            _dbContextHelper = new DbContextHelper(connectionString);
            _container.Register(Component.For<Core>().Instance(core));
            _container.Register(Component.For<DbContextHelper>().Instance(_dbContextHelper));
            _container.Install(
                new RebusHandlerInstaller()
                , new RebusInstaller(dbPrefix, connectionString, 1, 1, rabbitMqUser, rabbitMqPassword, rabbitmqHost)
            );

            _bus = _container.Resolve<IBus>();
        }

        public IBus GetBus()
        {
            return _bus;
        }

        private OuterInnerResourcePnDbContext GetContext()
        {
            OuterInnerResourcePnContextFactory contextFactory = new OuterInnerResourcePnContextFactory();
            return contextFactory.CreateDbContext(new[] {_connectionString});

        }

        public WindsorContainer GetContainer()
        {
            return (WindsorContainer)_container;
        }
    }
}