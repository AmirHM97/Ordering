using System;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Ordering.Application.Command;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Ordering.Application.Event.Integration
{
    public class BasketCompletedEventConsumer : IConsumer<BasketCompletedEvent>, IBusConsumerType
    {
        private readonly IMediator _mediator;

        public BasketCompletedEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<BasketCompletedEvent> context)
        {
            await _mediator.Publish(new CreateOrderCommand(context.Message));
        }
    }
}