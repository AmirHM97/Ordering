using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Product.Stock;
using MassTransit;
using MassTransit.RabbitMqTransport.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Query
{
    public class GetOrderStockItemsQueryConsumer : IConsumer<GetOrderStockItemsQuery>, IMediatorConsumerType
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Order> _orders;
        private readonly IOrderingReadDbContext _readDbContext;
        public GetOrderStockItemsQueryConsumer(IUnitOfWork uow, IOrderingReadDbContext readDbContext)
        {
            _uow = uow;
            _orders = _uow.Set<Order>();
            _readDbContext = readDbContext;
        }

        public async Task Consume(ConsumeContext<GetOrderStockItemsQuery> context)
        {
            //var stockIds = await _orders.Where(w => w.Id == context.Message.OrderId).Select(s=>s.OrderItems.Select(ss=>ss.StockId)).ToListAsync();
            try
            {
                var collection = _readDbContext.OrderCollection;
                var order = await collection
                        .Find(f => f.OrderId == context.Message.OrderId)
                        .FirstOrDefaultAsync();
                if (order != null)
                {
                    await context.RespondAsync(new GetOrderStockItemsQueryResponse
                    {
                        OrderId = context.Message.OrderId,
                        UserId = order.UserId,
                        TotalPrice = order.TotalPrice,
                        CheckStockItems = order.OrderItems.Select(s => new CheckStockItem
                        {
                            StockId = s.StockId,
                            RequestedCount = s.Quantity
                        }).ToList()
                    });
                }
                else
                {
                    await context.RespondAsync(new GetOrderStockItemsQueryResponse());
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public class GetOrderStockItemsQueryResponse
        {
            public long OrderId { get; set; }
            public string UserId { get; set; }
            public List<CheckStockItem> CheckStockItems { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}
