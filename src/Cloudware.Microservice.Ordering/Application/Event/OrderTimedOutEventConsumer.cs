using System.Threading.Tasks;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.BusTools;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using src.Ordering;

namespace Cloudware.Microservice.Ordering.Application.Event
{
    public class OrderTimedOutEventConsumer : IConsumer<OrderTimedOutEvent>, IBusConsumerType
    {
        private readonly IOrderService _orderService;
        private readonly IPublishEndpointInSingletonService _publishEndpointInSingletonService;

        public OrderTimedOutEventConsumer(IOrderService orderService, IPublishEndpointInSingletonService publishEndpointInSingletonService)
        {
            _orderService = orderService;
            _publishEndpointInSingletonService = publishEndpointInSingletonService;
        }

        public async Task Consume(ConsumeContext<OrderTimedOutEvent> context)
        {

            await _orderService.UpdateOrderStatus(context.Message.OrderId, OrderStatus.Canceled);
            await _publishEndpointInSingletonService.Publish(new OrdersCanceledEvent(context.Message.OrderId));
        }
    }
}