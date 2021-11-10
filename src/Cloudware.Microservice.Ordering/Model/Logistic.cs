using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Model
{
    public class Logistic
    {
        public long Id { get; set; }
        public List<Order>Orders { get; set; }

    }
    public class LogisticTimeitem
    {
        public long Id { get; set; }

    }
}
