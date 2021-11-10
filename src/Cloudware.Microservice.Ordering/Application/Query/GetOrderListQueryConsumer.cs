using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Microservice.Ordering.Model.ReadDb;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using MassTransit;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Query
{
    public class GetOrderListQueryConsumer : IConsumer<GetOrderListQuery>, IMediatorConsumerType
    {
        private readonly IOrderingReadDbContext _readDbContext;
        public GetOrderListQueryConsumer(IOrderingReadDbContext readDbContext)
        {
            _readDbContext = readDbContext;
        }
        public async Task Consume(ConsumeContext<GetOrderListQuery> context)
        {
            var collection = _readDbContext.OrderCollection;
            var orders = await collection.Find(f => f.OrderStatus==OrderStatus.Processing).ToListAsync();

            await context.RespondAsync(new GetOrderListQueryResponse(orders));
        }

        public class GetOrderListQueryResponse
        {
            public GetOrderListQueryResponse(List<OrderCollection> orders)
            {
                Orders = orders;
            }

            public List<OrderCollection> Orders { get; set; }
        }

        //public class GetOrderQueryResponse
        //{
        //    public long OrderId { get; set; }
        //    public string OrderCode { get; set; }
        //    public DateTime CreatedDate { get; set; }
        //    public decimal TotalPrice { get; set; }
        //    public OrderStatus OrderStatus { get; set; }
        //    public List<GetOrdersListQueryResponseItems> OrderItems { get; set; } = new();
        //}
        //public class GetOrdersListQueryResponseItems
        //{
        //    public string PictureUrl { get; set; }

        //}
    }
}
