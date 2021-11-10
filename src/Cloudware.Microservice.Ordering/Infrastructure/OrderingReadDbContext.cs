using Cloudware.Microservice.Ordering.Model.ReadDb;
using Cloudware.Utilities.Common.Setting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Infrastructure
{
    public class OrderingReadDbContext : IOrderingReadDbContext
    {
        private readonly IMongoDatabase _database = null;
        public OrderingReadDbContext(AppSettings settings)
        {
            var mongoUrl = MongoUrl.Create(settings.ConnectionStrings.MongoDb.Remove(settings.ConnectionStrings.MongoDb.LastIndexOf('/')));
            var client = new MongoClient(mongoUrl);
            _database = client.GetDatabase(settings.ConnectionStrings.MongoDb.Split('/')[3].Replace(".", "-"));
        }
        public IMongoCollection<OrderCollection> OrderCollection
        {
            get
            {
                return _database.GetCollection<OrderCollection>("OrderCollection");
            }
        }
        public IMongoCollection<OrderCollectionTest> OrderCollectionTest
        {
            get
            {
                return _database.GetCollection<OrderCollectionTest>("OrderCollection");
            }
        }
    }
}
