using Rebus.Bus;

namespace OuterInnerResource.Pn.Abstractions
{
    public interface IRebusService
    {
        void Start(string connectionString);
        IBus GetBus();
    }
}