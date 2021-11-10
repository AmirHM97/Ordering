using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Infrastructure
{
    public class OrderingSettings
    {
        public string OrderingConnectionString { get; set; }
        public string MongoConnectionString { get; set; }
        public string MongoDatabase { get; set; }
    }
}
