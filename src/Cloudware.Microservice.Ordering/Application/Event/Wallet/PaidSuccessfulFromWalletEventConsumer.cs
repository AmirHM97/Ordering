using Cloudware.Microservice.Ordering.Application.Command;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Wallet;
using MassTransit;
using MassTransit.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Cloudware.Microservice.Ordering.Application.Command.SetPaidOrderStatusCommandConsumer;


namespace Cloudware.Microservice.Ordering.Application.Event.Wallet
{
    public class PaidSuccessfulFromWalletEventConsumer : IConsumer<PaidSuccessfulFromWalletEvent>, IBusConsumerType
    {
        //     private readonly IRequestClient<SetPaidOrderStatusCommand> _setPaidOrderStatusCommand;

        //     public PaidSuccessfulFromWalletEventConsumer(IRequestClient<SetPaidOrderStatusCommand> setPaidOrderStatusCommand, IMediator mediator)
        //     {
        //         _setPaidOrderStatusCommand = setPaidOrderStatusCommand;
        //         _mediator = mediator;
        //     }
        private readonly IMediator _mediator;

        public PaidSuccessfulFromWalletEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<PaidSuccessfulFromWalletEvent> context)
        {

             await _mediator.Publish(new SetPaidOrderStatusCommand(context.Message.TenantId,context.Message.OrderId));
            //publish product sold event

            //await context.RespondAsync(new PaidSuccessfullFromWalletEventResponse { PaidSuccessfull = result.Message.Finished });
        }
        // public class PaidSuccessfullFromWalletEventResponse
        // {
        //     public bool PaidSuccessfull { get; set; }
        // }
    }
}
