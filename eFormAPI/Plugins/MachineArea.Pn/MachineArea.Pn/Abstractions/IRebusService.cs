using Rebus.Bus;

namespace MachineArea.Pn.Abstractions
{
    public interface IRebusService
    {
        void Start(string connectionString);
        IBus GetBus();
    }
}