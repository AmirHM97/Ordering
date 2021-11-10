using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Microservice.Ordering.Model.ReadDb;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Query
{
    public class GetOrdersListByUserIdQueryConsumer : IConsumer<GetOrdersListByUserIdQuery>, IMediatorConsumerType
    {
        private readonly IOrderingReadDbContext _readDbContext;
        private readonly ILogger<GetOrdersListByUserIdQueryConsumer> _logger;
        private readonly IPaginationService _paginationService;

        public GetOrdersListByUserIdQueryConsumer( ILogger<GetOrdersListByUserIdQueryConsumer> logger, IPaginationService paginationService, IOrderingReadDbContext readDbContext)
        {
            _logger = logger;
            _paginationService = paginationService;
            _readDbContext = readDbContext;
        }
        public async Task Consume(ConsumeContext<GetOrdersListByUserIdQuery> context)
        {
            try
            {

                var collection = _readDbContext.OrderCollection;
                var userOrders = await collection.Find(f => f.UserId == context.Message.UserId && f.OrderStatus == context.Message.OrderStatus)
                    .Project(p => new GetOrdersListByUserIdQueryResponse
                    {
                        OrderId = p.OrderId,
                        CreatedDate = p.CreatedDate,
                        OrderCode = p.OrderId.ToString(),
                        OrderStatus = p.OrderStatus,
                        TotalPrice = p.TotalPrice,
                        OrderItems = p.OrderItems.Select(s => new GetOrdersListByUserIdQueryResponseItems { PictureUrl = s.PictureUrl }).ToList()
                    }).ToListAsync();
                if (userOrders.Count == 0)
                {
                    await context.RespondAsync(new GetOrdersListByUserIdQueryResponse());
                    _logger.LogInformation($"no orders with status {context.Message.OrderStatus} found for user {context.Message.UserId} ");

                }
                else
                {
                    int skip = (context.Message.PageNumber - 1) * context.Message.PageSize;
                    _paginationService.Pagination(userOrders.AsQueryable(), context.Message.PageSize);
                    await context.RespondAsync(userOrders.Skip(skip).Take(context.Message.PageSize).ToList());
                }
            }
            catch (Exception e)
            {
                await context.RespondAsync(new GetOrdersListByUserIdQueryResponse());
                _logger.LogError($"error occurred in GetOrdersListByUserIdQueryConsumer {e.Message} ");

            }
        }
        public class GetOrdersListByUserIdQueryResponse
        {
            //   public List<OrderCollection> Orders { get; set; } = new List<OrderCollection>();

            public long OrderId { get; set; }
            public string OrderCode { get; set; }
            public DateTimeOffset CreatedDate { get; set; }
            public decimal TotalPrice { get; set; }
            public OrderStatus OrderStatus { get; set; }
            public List<GetOrdersListByUserIdQueryResponseItems> OrderItems { get; set; } = new();

            //public GetOrdersListByUserIdQueryResponse(List<OrderCollection> orders)
            //{
            //    Orders = orders;
            //}
        }
        public class GetOrdersListByUserIdQueryResponseItems
        {
            public string PictureUrl { get; set; }

        }
    }
}
