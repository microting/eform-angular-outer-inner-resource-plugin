using System.Threading.Tasks;
using Rebus.Bus;

namespace OuterInnerResource.Pn.Abstractions
{
    public interface IRebusService
    {
        Task Start(string connectionString);
        IBus GetBus();
    }
}