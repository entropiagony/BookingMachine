
using MongoDB.Driver;
using ReportMachine.Domain.Entities;

namespace ReportMachine.Domain
{
    public class ReportContext
    {
        IMongoDatabase database;
        public ReportContext()
        {
            string connectionString = "mongodb://localhost:27017/reportsdatabase";
            var connection = new MongoUrlBuilder(connectionString);
            MongoClient client = new MongoClient(connectionString);
            database = client.GetDatabase(connection.DatabaseName);
        }

        public IMongoCollection<Report> Reports
        {
            get { return database.GetCollection<Report>("Reports"); }
        }
    }
}
