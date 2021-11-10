using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Query
{
    public class GetOrderDetailStatusQuery:IRequestType
    {
        public long OrderId { get; set; }

        public GetOrderDetailStatusQuery(long orderId)
        {
            OrderId = orderId;
        }
    }
}
