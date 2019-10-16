using Castle.MicroKernel.Registration;
using Castle.Windsor;
using eFormCore;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Factories;
using OuterInnerResource.Pn.Abstractions;
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

        public RebusService(IEFormCoreService coreHelper)
        {            
            //_dbContext = dbContext;
            _coreHelper = coreHelper;
        }

        public void Start(string connectionString)
        {
            _connectionString = connectionString;   
            _container = new WindsorContainer();
            _container.Install(
                new RebusHandlerInstaller()
                , new RebusInstaller(connectionString, 1, 1)
            );
            
            Core _core = _coreHelper.GetCore();
            _container.Register(Component.For<Core>().Instance(_core));
            _container.Register(Component.For<OuterInnerResourcePnDbContext>().Instance(GetContext()));
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