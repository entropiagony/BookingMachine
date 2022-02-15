using MongoDB.Driver;
using ReportMachine.Domain;
using ReportMachine.Domain.Entities;

namespace ReportMachine.Repository.Interfaces
{
    public interface IReportRepository
    {
        public Task CreateReport(Report report);
        public Task UpdateReport(Report report);
        public Task<IFindFluent<Report, Report>> GetReports(ReportFilter filter);
        public Task<Report> GetReport(int id);
    }
}
