using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.Contract.Notification;
using MassTransit;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Ordering.Application.Query
{
    public class GetOrderStatusFiltersQueryConsumer : IConsumer<GetOrderStatusFiltersQuery>, IMediatorConsumerType
    {
        private readonly IOrderingReadDbContext _readDbContext;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Order> _orders;
        public GetOrderStatusFiltersQueryConsumer(IUnitOfWork uow, IOrderingReadDbContext readDbContext)
        {
            _uow = uow;
            _orders = _uow.Set<Order>();
            _readDbContext = readDbContext;
        }

        public async Task Consume(ConsumeContext<GetOrderStatusFiltersQuery> context)
        {
            var collection = _readDbContext.OrderCollection;

            var orderStatusCountList = await collection.Aggregate()
                 .Match(m => m.UserId == context.Message.UserId)
                 .Group(g => g.OrderStatus, g => new { g.Key, Count = g.Count() }).ToListAsync();

            #region comment
            //var orderStatusCountDictionary = await _orders.Where(w => w.UserId == context.Message.UserId)
            //    .GroupBy(g => g.OrderStatus)
            //    .Select(s => new { s.Key, Count = s.Count()})
            //    .ToDictionaryAsync(x => x.Key, x => x.Count);

            //var orderStatusItems = new List<GetOrderStatusFiltersQueryResponseItem>();
            //orderStatusItems.Add(new GetOrderStatusFiltersQueryResponseItem
            //{
            //    StatusId = (int)OrderStatus.WaitingForPayment,
            //    StatusName = "در انتظار پرداخت",
            //    OrdersCount = orderStatusCountDictionary.FirstOrDefault(f => f.Key == OrderStatus.WaitingForPayment).Value
            //});
            //orderStatusItems.Add(new GetOrderStatusFiltersQueryResponseItem
            //{
            //    StatusId = (int)OrderStatus.Procesing,
            //    StatusName = "در حال پردازش",
            //    OrdersCount = orderStatusCountDictionary.FirstOrDefault(f => f.Key == OrderStatus.Procesing).Value
            //});
            //orderStatusItems.Add(new GetOrderStatusFiltersQueryResponseItem
            //{
            //    StatusId = (int)OrderStatus.Delivered,
            //    StatusName = "تحویل شده",
            //    OrdersCount = orderStatusCountDictionary.FirstOrDefault(f => f.Key == OrderStatus.Delivered).Value

            //});
            //orderStatusItems.Add(new GetOrderStatusFiltersQueryResponseItem
            //{
            //    StatusId = (int)OrderStatus.Canceled,
            //    StatusName = "مرجوعی",
            //    OrdersCount = orderStatusCountDictionary.FirstOrDefault(f => f.Key == OrderStatus.Canceled).Value

            //}); 
            #endregion
            var orderStatusItems = new List<GetOrderStatusFiltersQueryResponseItem>
            {
                new GetOrderStatusFiltersQueryResponseItem
                {
                    StatusId = (int)OrderStatus.WaitingForPayment,
                    StatusName = "در انتظار پرداخت",
                    OrdersCount = orderStatusCountList.Where(f => f.Key == OrderStatus.WaitingForPayment).Select(s => s.Count).FirstOrDefault()
                },
                new GetOrderStatusFiltersQueryResponseItem
                {
                    StatusId = (int)OrderStatus.Processing,
                    StatusName = "در حال پردازش",
                    OrdersCount = orderStatusCountList.Where(f => f.Key == OrderStatus.Processing || f.Key == OrderStatus.Paid).Select(s => s.Count).FirstOrDefault()
                },
                new GetOrderStatusFiltersQueryResponseItem
                {
                    StatusId = (int)OrderStatus.Delivered,
                    StatusName = "تحویل شده",
                    OrdersCount = orderStatusCountList.Where(f => f.Key == OrderStatus.Delivered).Select(s => s.Count).FirstOrDefault()

                },
                new GetOrderStatusFiltersQueryResponseItem
                {
                    StatusId = (int)OrderStatus.Canceled,
                    StatusName = "مرجوعی",
                    OrdersCount = orderStatusCountList.Where(f => f.Key == OrderStatus.Canceled).Select(s => s.Count).FirstOrDefault()

                }
            };

            await context.RespondAsync(new GetOrderStatusFiltersQueryResponse(orderStatusItems));
        }
        public class GetOrderStatusFiltersQueryResponse
        {
            public List<GetOrderStatusFiltersQueryResponseItem> OrderStatusItems { get; set; } = new List<GetOrderStatusFiltersQueryResponseItem>();

            public GetOrderStatusFiltersQueryResponse(List<GetOrderStatusFiltersQueryResponseItem> orderStatusItems)
            {
                OrderStatusItems = orderStatusItems;
            }
        }
        public class GetOrderStatusFiltersQueryResponseItem
        {
            public int StatusId { get; set; }
            public string StatusName { get; set; }
            public long OrdersCount { get; set; } = 0;
        }
    }
}
