using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Model
{
    public class Address
    {
        public long Id { get; set; }
     
        public virtual ICollection<Order>  Orders { get; set; }
    }
}
