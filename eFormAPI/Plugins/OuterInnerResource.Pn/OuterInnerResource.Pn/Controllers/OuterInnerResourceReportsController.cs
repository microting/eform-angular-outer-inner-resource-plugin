using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Models;
using OuterInnerResource.Pn.Infrastructure.Models.Report;

namespace OuterInnerResource.Pn.Controllers
{
    [Authorize]
    public class OuterInnerResourceReportsController : Controller
    {
        private readonly IOuterInnerResourceReportService _outerInnerResourceReportService;

        public OuterInnerResourceReportsController(IOuterInnerResourceReportService outerInnerResourceReportService)
        {
            _outerInnerResourceReportService = outerInnerResourceReportService;
        }


        [HttpGet]
        [Route("api/outer-inner-resource-pn/reports/reportnames")]
        [AllowAnonymous]
        public OperationDataResult<ReportNamesModel> GetReportNames()
        {
            return _outerInnerResourceReportService.GetReportNames();
        }

        [HttpGet]
        [Route("api/outer-inner-resource-pn/reports")]
        public async Task<OperationDataResult<ReportModel>> GenerateReport(GenerateReportModel requestModel)
        {
            return await _outerInnerResourceReportService.GenerateReport(requestModel);
        }

        /// <summary>
        /// Download records export excel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns code="200">Return excel blob</returns>
        /// <returns code="400">Error message</returns>
        [HttpGet]
        [Route("api/outer-inner-resource-pn/reports/excel")]
        [ProducesResponseType(typeof(string), 400)]
        public async Task GenerateReportFile(GenerateReportModel requestModel)
        {
            OperationDataResult<FileStreamModel> result = await _outerInnerResourceReportService.GenerateReportFile(requestModel);
            const int bufferSize = 4086;
            byte[] buffer = new byte[bufferSize];
            Response.OnStarting(async () =>
            {
                try
                {
                    if (!result.Success)
                    {
                        Response.ContentLength = result.Message.Length;
                        Response.ContentType = "text/plain";
                        Response.StatusCode = 400;
                        byte[] bytes = Encoding.UTF8.GetBytes(result.Message);
                        await Response.Body.WriteAsync(bytes, 0, result.Message.Length);
                        await Response.Body.FlushAsync();
                    }
                    else
                    {
                        using (FileStream excelStream = result.Model.FileStream)
                        {
                            int bytesRead;
                            Response.ContentLength = excelStream.Length;
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            while ((bytesRead = excelStream.Read(buffer, 0, buffer.Length)) > 0 &&
                                   !HttpContext.RequestAborted.IsCancellationRequested)
                            {
                                await Response.Body.WriteAsync(buffer, 0, bytesRead);
                                await Response.Body.FlushAsync();
                            }
                        }
                    
                    }
                }
                finally
                {
                    if (!string.IsNullOrEmpty(result?.Model?.FilePath)
                        && System.IO.File.Exists(result.Model.FilePath))
                    {
                        System.IO.File.Delete(result.Model.FilePath);
                    }
                }
            });
        }
    }
}