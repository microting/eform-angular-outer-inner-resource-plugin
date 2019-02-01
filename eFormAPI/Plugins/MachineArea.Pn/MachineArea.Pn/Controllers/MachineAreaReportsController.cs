using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
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

        public MachineAreaReportsController(IMachineAreaReportService machineAreaReportService)
        {
            _machineAreaReportService = machineAreaReportService;
        }

        [HttpGet]
        [Route("api/machine-area-pn/reports")]
        public async Task<OperationDataResult<ReportModel>> GenerateReport(GenerateReportModel requestModel)
        {
            return await _machineAreaReportService.GenerateReport(requestModel);
        }

        [HttpGet]
        [Route("api/machine-area-pn/reports/excel")]
        public async Task<OperationResult> GenerateReportFile(GenerateReportModel requestModel)
        {
            return await _machineAreaReportService.GenerateReportFile(requestModel);
        }
    }
}