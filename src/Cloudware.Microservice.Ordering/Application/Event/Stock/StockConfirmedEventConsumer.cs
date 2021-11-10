using Cloudware.Microservice.Ordering.Application.Command;
using Cloudware.Microservice.Ordering.Application.Command.Status;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.BusTools;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Ordering;
using Cloudware.Utilities.Contract.Product;
using Contracts;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Cloudware.Microservice.Ordering.Application.Command.CreateOrderCommandConsumer;

namespace Cloudware.Microservice.Ordering.Application.Event.Stock
{
    public class StockConfirmedEventConsumer : IConsumer<StockConfirmedEvent>, IBusConsumerType
    {
        private readonly IPublishEndpointInSingletonService _publishEndpoint;
        private readonly IMediator _mediator;
        private readonly ILogger<StockConfirmedEventConsumer> _logger;
        private readonly IOrderService _orderService;

        public StockConfirmedEventConsumer(IPublishEndpointInSingletonService publishEndpoint, IMediator mediator, ILogger<StockConfirmedEventConsumer> logger, IOrderService orderService)
        {
            _publishEndpoint = publishEndpoint;
            _mediator = mediator;
            _logger = logger;
            _orderService = orderService;
        }
        public async Task Consume(ConsumeContext<StockConfirmedEvent> context)
        {
            _logger.LogInformation($" createOrderCommand has been Called !!!");
            //TODO: Update orderStatus
            await _orderService.UpdateOneStatusAsync(context.Message.TenantId,context.Message.OrderId, OrderStatus.StockConfirmed);
            var statusList = new List<OrderStatus>{
                OrderStatus.StockConfirmed,
                OrderStatus.addressConfirmed
            };
            var res = await _orderService.CheckExistingStatus(context.Message.OrderId, statusList);
            if (res)
            {
                await _mediator.Publish(new SetWaitingForPaymentOrderStatusCommand(context.Message.OrderId,context.Message.TenantId));

                await _mediator.Publish(new PublishOrderCreatedCompletelyEventCommand(context.Message.OrderId,context.Message.TenantId));
            }

        }
    }
}
