using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.Machines
{
    public class MachinesModel
    {
        public int Total { get; set; }
        public List<MachineModel> MachineList { get; set; }
        public string Name { get; set; }
    }
}