using System;
using System.Threading.Tasks;
using Microting.eFormMachineAreaBase.Infrastructure.Data;

namespace MachineArea.Pn.Infrastructure.Models
{
    [Obsolete]
    interface IModel
    {
        Task Save(MachineAreaPnDbContext dbContext);

        Task Update(MachineAreaPnDbContext dbContext);

        Task Delete(MachineAreaPnDbContext dbContext);
    }
}