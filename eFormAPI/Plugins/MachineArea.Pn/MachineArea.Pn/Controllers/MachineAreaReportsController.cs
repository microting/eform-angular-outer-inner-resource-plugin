using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.Areas;
using MachineArea.Pn.Infrastructure.Models.Machines;
using MachineArea.Pn.Infrastructure.Models.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Controllers
{
    [Authorize]
    public class MachineAreaReportsController : Controller
    {
        private readonly IMachineAreaReportService _machineAreaReportService;

        public MachinesController(IMachineAreaReportService machineAreaReportService)
        {
            _machineAreaReportService = _machineAreaReportService;
        }

        [HttpGet]
        [Route("api/machine-area-pn/reports")]
        public async Task<OperationDataResult<ReportModel>> GenerateReport(GenerateReportModel requestModel)
        {
            return await _machineAreaReportService.GenerateReport(requestModel);
        }

        [HttpGet]
        [Route("api/machine-area-pn/reports/excel")]
        public async Task<OperationDataResult<MachineModel>> GenerateReportFile(GenerateReportModel requestModel)
        {
            return await _machineAreaReportService.GenerateReportFile(requestModel);
        }
    }
}