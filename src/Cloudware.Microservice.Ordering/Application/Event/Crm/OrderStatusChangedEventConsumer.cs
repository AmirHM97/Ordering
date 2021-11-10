using System.Threading.Tasks;
using Cloudware.Microservice.Ordering.Application.Command.Status;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
using src.Ordering.Crm;

namespace Cloudware.Microservice.Ordering.Application.Event.Crm
{
    public class OrderStatusChangedEventConsumer : IConsumer<OrderStatusChangedEvent>, IBusConsumerType
    {
        private readonly IMediator _mediator;

        public OrderStatusChangedEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<OrderStatusChangedEvent> context)
        {
           await _mediator.Publish(new ChangeOrderStatusCommand(context.Message.TenantId,context.Message.OrderId,(OrderStatus)context.Message.Status));
        }
    }
}