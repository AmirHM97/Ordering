using System.Threading.Tasks;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;

namespace Cloudware.Microservice.Ordering.Application.Command.Status
{
    public class SetWaitingForPaymentOrderStatusCommandCosumer : IConsumer<SetWaitingForPaymentOrderStatusCommand>, IMediatorConsumerType
    {
        private readonly IOrderService _orderService;

        public SetWaitingForPaymentOrderStatusCommandCosumer(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task Consume(ConsumeContext<SetWaitingForPaymentOrderStatusCommand> context)
        {
            await _orderService.UpdateOneStatusAsync(context.Message.TenantId, context.Message.OrderId, OrderStatus.WaitingForPayment);

        }
    }
}