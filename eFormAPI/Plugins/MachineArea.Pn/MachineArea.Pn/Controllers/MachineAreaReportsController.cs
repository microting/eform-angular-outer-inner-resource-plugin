using System.Text;
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

        /// <summary>
        /// Download records export excel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns code="200">Return excel blob</returns>
        /// <returns code="400">Error message</returns>
        [HttpGet]
        [Route("api/machine-area-pn/reports/excel")]
        [ProducesResponseType(typeof(string), 400)]
        public async Task GenerateReportFile(GenerateReportModel requestModel)
        {
            var result = await _machineAreaReportService.GenerateReportFile(requestModel);
            const int bufferSize = 4086;
            var buffer = new byte[bufferSize];
            Response.OnStarting(async () =>
            {
                if (!result.Success)
                {
                    Response.ContentLength = result.Message.Length;
                    Response.ContentType = "text/plain";
                    Response.StatusCode = 400;
                    var bytes = Encoding.UTF8.GetBytes(result.Message);
                    await Response.Body.WriteAsync(bytes, 0, result.Message.Length);
                    await Response.Body.FlushAsync();
                }
                else
                {
                    using (var excelStream = result.Model.FileStream)
                    {
                        int bytesReaded;
                        Response.ContentLength = excelStream.Length;
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        while ((bytesReaded = excelStream.Read(buffer, 0, buffer.Length)) > 0 &&
                               !HttpContext.RequestAborted.IsCancellationRequested)
                        {
                            await Response.Body.WriteAsync(buffer, 0, bytesReaded);
                            await Response.Body.FlushAsync();
                        }
                    }
                    System.IO.File.Delete(result.Model.FilePath);
                }
            });
        }
    }
}