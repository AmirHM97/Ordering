using System.Threading.Tasks;
using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;

namespace Cloudware.Microservice.Ordering.Application.Command.Status
{
    public class ChangeOrderStatusCommandConsumer : IConsumer<ChangeOrderStatusCommand>, IMediatorConsumerType
    {
        private readonly IMediator _mediator;
        private readonly IOrderingReadDbContext _readDbContext;
        private readonly IOrderService _orderService;


        public ChangeOrderStatusCommandConsumer(IMediator mediator, IOrderService orderService, IOrderingReadDbContext readDbContext)
        {
            _mediator = mediator;
            _orderService = orderService;
            _readDbContext = readDbContext;
        }

        public async Task Consume(ConsumeContext<ChangeOrderStatusCommand> context)
        {
            switch (context.Message.OrderStatus)
            {
                case Model.OrderStatus.WaitingForPayment:
                    await _mediator.Publish(new SetWaitingForPaymentOrderStatusCommand(context.Message.OrderId, context.Message.TenantId));
                    break;
                case Model.OrderStatus.Paid:
                    await _mediator.Publish(new SetPaidOrderStatusCommand(context.Message.TenantId, context.Message.OrderId));
                    break;
                case Model.OrderStatus.Canceled:
                    await _mediator.Publish(new SetCanceledOrderStatusCommand(context.Message.TenantId, context.Message.OrderId));
                    break;
                case Model.OrderStatus.Processing:
                case Model.OrderStatus.PaymentFailed:
                case Model.OrderStatus.StockConfirmed:
                case Model.OrderStatus.StockNotConfirmed:
                case Model.OrderStatus.Submitted:
                case Model.OrderStatus.AwaitingValidation:
                case Model.OrderStatus.Delivered:
                case Model.OrderStatus.init:
                case Model.OrderStatus.shippingPriceConfirmed:
                case Model.OrderStatus.addressConfirmed:
                case Model.OrderStatus.dataReceivedFromProduct:
                case Model.OrderStatus.preparing:
                case Model.OrderStatus.sending:
                    {
                        await _orderService.UpdateOrderStatus(context.Message.TenantId, context.Message.OrderId, context.Message.OrderStatus);
                        //! publish an event if it's needed !!!!
                    }
                    break;
                default:
                    break;
            }
        }
    }
}