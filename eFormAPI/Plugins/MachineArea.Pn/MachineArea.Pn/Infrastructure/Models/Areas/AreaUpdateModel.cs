using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.Areas
{
    public class AreaUpdateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RelateMachinesIds { get; set; }
    }
}