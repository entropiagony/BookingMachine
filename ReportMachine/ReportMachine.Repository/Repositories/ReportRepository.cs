using MongoDB.Bson;
using MongoDB.Driver;
using ReportMachine.Common.Pagination;
using ReportMachine.Domain;
using ReportMachine.Domain.Entities;
using ReportMachine.Repository.Interfaces;


namespace ReportMachine.Repository.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ReportContext db;

        public ReportRepository(ReportContext db)
        {
            this.db = db;
        }

        public async Task CreateReport(Report report)
        {
            await db.Reports.InsertOneAsync(report);
        }

        public async Task<Report> GetReport(int id)
        {
            return await db.Reports.Find(new BsonDocument("_id", id)).FirstOrDefaultAsync();
        }

        public async Task<IFindFluent<Report, Report>> GetReports(ReportFilter filter)
        {
            var dateQuery = new BsonDocument
            {
                {"BookingDate" , new BsonDocument {
                    { "$gte" , filter.DateFrom.Date},
                    { "$lte" , filter.DateTo.Date},
                }}
            };
            var floorQuery = filter.FloorNumber != -1 
                ? new BsonDocument("FloorNumber", filter.FloorNumber) : new BsonDocument();
            var resultQuery = new BsonDocument("$and", new BsonArray { dateQuery, floorQuery });

            return db.Reports.Find(resultQuery);

        }

        public async Task UpdateReport(Report report)
        {
            await db.Reports.ReplaceOneAsync(new BsonDocument("_id", report.Id), report);
        }


    }
}
