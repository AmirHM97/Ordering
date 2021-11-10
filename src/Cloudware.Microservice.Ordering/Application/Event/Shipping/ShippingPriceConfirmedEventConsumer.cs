using Cloudware.Microservice.Ordering.Application.Command;
using Cloudware.Microservice.Ordering.Application.Command.Status;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.BusTools;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Ordering;
using Cloudware.Utilities.Contract.Shipping;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Cloudware.Microservice.Ordering.Application.Command.CreateOrderCommandConsumer;

namespace Cloudware.Microservice.Ordering.Application.Event.Shipping
{
    public class ShippingPriceConfirmedEventConsumer : IConsumer<ShippingPriceConfirmedEvent>, IBusConsumerType
    {
        private readonly IOrderService _orderService;
        private readonly IMediator _mediator;
        private readonly IPublishEndpointInSingletonService _publishEndpoint;

        public ShippingPriceConfirmedEventConsumer(IOrderService orderService, IPublishEndpointInSingletonService publishEndpoint, IMediator mediator)
        {
            _orderService = orderService;
            _publishEndpoint = publishEndpoint;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ShippingPriceConfirmedEvent> context)
        {
            await _orderService.UpdateShippingPrice(context.Message.TenantId,context.Message.OrderId, context.Message.ShippingPrice);
            await _orderService.UpdateOneStatusAsync(context.Message.TenantId,context.Message.OrderId, OrderStatus.shippingPriceConfirmed);
            var statusList = new List<OrderStatus>{
                OrderStatus.StockConfirmed,
                OrderStatus.addressConfirmed
            };
            var res = await _orderService.CheckExistingStatus(context.Message.OrderId, statusList);
            if (res)
            {
                await _mediator.Publish(new SetWaitingForPaymentOrderStatusCommand(context.Message.OrderId, context.Message.TenantId));
                
                await _mediator.Publish(new PublishOrderCreatedCompletelyEventCommand(context.Message.OrderId,context.Message.TenantId));
            }
        }
    }
}
