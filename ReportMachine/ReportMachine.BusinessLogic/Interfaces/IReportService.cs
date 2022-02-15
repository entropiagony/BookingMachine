using ReportMachine.Common.Pagination;
using ReportMachine.Domain;
using ReportMachine.Domain.Entities;

namespace ReportMachine.BusinessLogic.Interfaces
{
    public interface IReportService
    {
        public Task<PagedList<Report>> GetReports(ReportFilter filter, int pageNumber, int pageSize);

    }
}
