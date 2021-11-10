using System.Threading.Tasks;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;

namespace Cloudware.Microservice.Ordering.Application.Command.Status
{
    public class SetCanceledOrderStatusCommandConsumer : IConsumer<SetCanceledOrderStatusCommand>, IMediatorConsumerType
    {
        private readonly IOrderService _orderService;

        public SetCanceledOrderStatusCommandConsumer(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task Consume(ConsumeContext<SetCanceledOrderStatusCommand> context)
        {
            await _orderService.UpdateOneStatusAsync(context.Message.TenantId,context.Message.OrderId, OrderStatus.Canceled);
        }
    }
}