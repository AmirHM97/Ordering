
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Query
{
    public class GetOrderByIdQuery:IRequestType
    {
        public long  OrderId { get; set; }
        public bool  StatusNeeded { get; set; }


        public GetOrderByIdQuery(long orderId, bool statusNeeded)
        {
            OrderId = orderId;
            StatusNeeded = statusNeeded;
        }
    }
}
