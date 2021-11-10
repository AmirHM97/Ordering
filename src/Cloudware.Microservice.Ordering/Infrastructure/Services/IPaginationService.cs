using System.Linq;

namespace Cloudware.Microservice.Ordering.Infrastructure.Services
{
    public interface IPaginationService
    {
        object Pagination<T>(IQueryable<T> items, int pageSize);
    }
}