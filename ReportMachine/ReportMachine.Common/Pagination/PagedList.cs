
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ReportMachine.Common.Pagination
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, long count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public long TotalCount { get; set; }

        public async static Task<PagedList<T>> CreateAsync(IFindFluent<T, T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountDocumentsAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Limit(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
