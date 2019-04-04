using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eFormShared;
using Microsoft.CodeAnalysis;
using Microting.eFormMachineAreaBase.Infrastructure.Data;
using Microting.eFormMachineAreaBase.Infrastructure.Data.Entities;

namespace MachineArea.Pn.Infrastructure.Models.Machines
{
    public class MachineModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RelatedAreasIds { get; set; }
    }
}
