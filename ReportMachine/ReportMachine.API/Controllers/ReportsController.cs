using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReportMachine.API.Extensions;
using ReportMachine.BusinessLogic.Interfaces;
using ReportMachine.Common.Pagination;
using ReportMachine.Domain;
using ReportMachine.Domain.Entities;

namespace ReportMachine.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService reportService;

        public ReportsController(IReportService reportService)
        {
            this.reportService = reportService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<Report>>> GetAllReports([FromQuery] ReportFilter filter, 
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var reports = await reportService.GetReports(filter, pageNumber, pageSize);
            Response.AddPaginationHeader(reports.CurrentPage, reports.PageSize, reports.TotalCount, reports.TotalPages);
            return Ok(reports);
        }
    }
}
