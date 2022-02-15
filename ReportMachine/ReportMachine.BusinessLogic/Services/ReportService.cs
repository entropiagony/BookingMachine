using ReportMachine.BusinessLogic.Interfaces;
using ReportMachine.Common.Pagination;
using ReportMachine.Domain;
using ReportMachine.Domain.Entities;
using ReportMachine.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMachine.BusinessLogic.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            this.reportRepository = reportRepository;
        }


        public async Task<PagedList<Report>> GetReports(ReportFilter filter, int pageNumber, int pageSize)
        {
            var reports = await reportRepository.GetReports(filter);
            var result = await PagedList<Report>.CreateAsync(reports, pageNumber, pageSize);
            return result;
        }
    }
}
