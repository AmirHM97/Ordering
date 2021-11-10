using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Ordering;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Query.Integration
{
    public class GetOrderByCorrelationIdQueryConsumer : IConsumer<GetOrderByCorrelationIdQuery>, IBusConsumerType
    {
        // private readonly DbSet<Order> _orders;
        // private readonly IUnitOfWork _uow;

        // public GetOrderByCorrelationIdQueryConsumer(IUnitOfWork uow)
        // {
        //     _uow = uow;
        //     _orders = _uow.Set<Order>();
        // }
        private readonly IOrderService _orderService;

        public GetOrderByCorrelationIdQueryConsumer(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task Consume(ConsumeContext<GetOrderByCorrelationIdQuery> context)
        {
            var order = await _orderService.GetOneByCorrelationId(context.Message.CorrelationId.ToString());
             if (order != null && order.OrderStatusLogs.Contains(OrderStatus.shippingPriceConfirmed) && order.OrderStatusLogs.Contains(OrderStatus.StockConfirmed)&&order.OrderStatusLogs.Contains(OrderStatus.addressConfirmed))
            {
                await context.RespondAsync(new GetOrderByCorrelationIdQueryResponse
                {
                    OrderId = order.OrderId,
                    TotalPrice = order.TotalPrice,
                    IsPaid = order.OrderStatus == OrderStatus.Paid
                });
            }
            else
            {
                  await context.RespondAsync(new GetOrderByCorrelationIdQueryResponse
                {
                    OrderId = 0,
                    TotalPrice = 0,
                    IsPaid =false
                });
              //  throw new AppException(5059, System.Net.HttpStatusCode.BadRequest, "order not made");
                // await context.RespondAsync(new GetOrderByCorrelationIdQueryResponse
                // {
                //     OrderId = 0,
                //     TotalPrice = 0,
                //     IsPaid = false
                // });
            }
        }

    }
}
