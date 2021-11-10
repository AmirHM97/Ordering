using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Query
{
    public class GetFilteredOrdersListOfUserByOrderStatusQuery:IRequestType
    {
        public GetFilteredOrdersListOfUserByOrderStatusQuery(string userId, OrderStatus orderStatus, int pageSize, int pageNumber)
        {
            UserId = userId;
            OrderStatus = orderStatus;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public string UserId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
