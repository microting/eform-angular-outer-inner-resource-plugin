using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using eFormCore;
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
        }

        public async Task Start(string connectionString, int maxParallelism, int numberOfWorkers)
        {
            _connectionString = connectionString;
            //_sdkConnectionString = sdkConnectionString;
            _container = new WindsorContainer();
            _container.Install(
                new RebusHandlerInstaller()
                , new RebusInstaller(connectionString, maxParallelism, numberOfWorkers)
            );
            
            Core core = await _coreHelper.GetCore();
            _dbContextHelper = new DbContextHelper(connectionString);
            
            _container.Register(Component.For<Core>().Instance(core));
            _container.Register(Component.For<DbContextHelper>().Instance(_dbContextHelper));
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
    }
}