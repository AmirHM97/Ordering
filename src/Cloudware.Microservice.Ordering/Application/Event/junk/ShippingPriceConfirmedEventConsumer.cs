using Cloudware.Microservice.Ordering.Application.Command;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Event
{
    //public class ShippingPriceConfirmedEventConsumer : IConsumer<ShippingPriceConfirmedEvent>
    //{
    //    private readonly IMediator _mediator;
    //    private readonly IRequestClient<CreateOrderCommand> _createOrderCommand;
    //    private readonly ILogger<ShippingPriceConfirmedEventConsumer> _logger;

    //    public ShippingPriceConfirmedEventConsumer(IMediator mediator, ILogger<ShippingPriceConfirmedEventConsumer> logger, IRequestClient<CreateOrderCommand> createOrderCommand)
    //    {
    //        _mediator = mediator;
    //        _logger = logger;
    //        _createOrderCommand = createOrderCommand;
    //    }

    //    public Task Consume(ConsumeContext<ShippingPriceConfirmedEvent> context)
    //    {
    //       // await _createOrderCommand.GetResponse<>();
    //    }

    //    public async Task Handle(UserCheckoutAcceptedEvent notification, CancellationToken cancellationToken)
    //    {

    //       await _mediator.Send(new CreateOrderCommand());
           
    //    }
    //}
}
