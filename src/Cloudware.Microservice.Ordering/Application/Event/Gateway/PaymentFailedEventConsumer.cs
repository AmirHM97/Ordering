using System.Threading.Tasks;
using Cloudware.Microservice.Ordering.Application.Command.Status;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.PaymentGatway;
using MassTransit;
using MassTransit.Mediator;

namespace Cloudware.Microservice.Ordering.Application.Event.Gateway
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>, IBusConsumerType
    {
        private readonly IMediator _mediator;

        public PaymentFailedEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            await _mediator.Publish(new SetCanceledOrderStatusCommand(context.Message.TenantId,context.Message.OrderId));
        }
    }
}