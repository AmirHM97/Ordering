using Cloudware.Microservice.Ordering.DTO;
using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
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
    public class GetFilteredOrdersListOfUserByOrderStatusQueryConsumer : IConsumer<GetFilteredOrdersListOfUserByOrderStatusQuery>, IMediatorConsumerType
    {
        private readonly IOrderingReadDbContext _readDbContext;
        private readonly IPaginationService _paginationService;
        private readonly ILogger<GetFilteredOrdersListOfUserByOrderStatusQueryConsumer> _logger;
        public GetFilteredOrdersListOfUserByOrderStatusQueryConsumer(ILogger<GetFilteredOrdersListOfUserByOrderStatusQueryConsumer> logger, IPaginationService paginationService, IOrderingReadDbContext readDbContext)
        {
            _logger = logger;
            _paginationService = paginationService;
            _readDbContext = readDbContext;
        }
        public async Task Consume(ConsumeContext<GetFilteredOrdersListOfUserByOrderStatusQuery> context)
        {
            var collection = _readDbContext.OrderCollection;
            int skip = (context.Message.PageNumber - 1) * context.Message.PageSize;

            var orders = collection.Find(f => f.UserId == context.Message.UserId && f.OrderStatus == context.Message.OrderStatus).SortByDescending(o => o.CreatedDate);

            _paginationService.Pagination(orders.ToEnumerable().AsQueryable(), context.Message.PageSize);
            if (orders.ToEnumerable().AsQueryable().Any())
            {
                var orderstWithPagination = await orders.Skip(skip).Limit(context.Message.PageSize).Project(p => new OrderGeneralInfoDto
                {
                    OrderId = p.OrderId,
                    CreatedDate = p.CreatedDate,
                    OrderItemImageUrls = p.OrderItems.Select(s => s.PictureUrl).ToList(),
                    TotalPrice = p.TotalPrice,
                    OrderStatus=p.OrderStatus
                }).ToListAsync();
                await context.RespondAsync(new GetFilteredOrdersListOfUserByOrderStatusQueryResponse(orderstWithPagination));
            }
            else
            {
                await context.RespondAsync(new GetFilteredOrdersListOfUserByOrderStatusQueryResponse(new()));

            }
        }
    }
    public class GetFilteredOrdersListOfUserByOrderStatusQueryResponse
    {

        public List<OrderGeneralInfoDto> Orders { get; set; }

        public GetFilteredOrdersListOfUserByOrderStatusQueryResponse(List<OrderGeneralInfoDto> orders)
        {
            Orders = orders;
        }
    }
}
