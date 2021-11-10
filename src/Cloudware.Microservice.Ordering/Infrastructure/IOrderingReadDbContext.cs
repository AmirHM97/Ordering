using Cloudware.Microservice.Ordering.Model.ReadDb;
using MongoDB.Driver;

namespace Cloudware.Microservice.Ordering.Infrastructure
{
    public interface IOrderingReadDbContext
    {
        IMongoCollection<OrderCollection> OrderCollection { get; }
        IMongoCollection<OrderCollectionTest> OrderCollectionTest { get; }
    }
}