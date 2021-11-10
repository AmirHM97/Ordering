using Cloudware.Microservice.Ordering.Application.Event;
using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Microservice.Ordering.Model.ReadDb;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Ordering;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.Mediator;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Command
{
    public class SetPaidOrderStatusCommandConsumer : IConsumer<SetPaidOrderStatusCommand>, IMediatorConsumerType
    {
        // private readonly IUnitOfWork _uow;
        // private readonly DbSet<Order> _order;
        private readonly ILogger<SetPaidOrderStatusCommandConsumer> _logger;
        // private readonly IOrderingReadDbContext _readDbContext;
        private readonly IMediator _mediator;
        private readonly IOrderService _orderService;
        public SetPaidOrderStatusCommandConsumer( ILogger<SetPaidOrderStatusCommandConsumer> logger, IMediator mediator, IOrderService orderService )
        {
            // _uow = uow;
            // _order = _uow.Set<Order>();
            _logger = logger;
            _mediator = mediator;
            _orderService = orderService;
            // _readDbContext = readDbContext;
        }

        public async Task Consume(ConsumeContext<SetPaidOrderStatusCommand> context)
        {
            var order = await _orderService.UpdateOrderStatus(context.Message.TenantId, context.Message.OrderId, OrderStatus.Paid);
            await context.RespondAsync(new SetPaidOrderStatusCommandResponse { Finished = true });
            await _mediator.Publish(new OrderStatusChangedToPaidEvent(context.Message.OrderId, order.UserId, order.TenantId));
            _logger.LogInformation($"order {context.Message.OrderId}'s Status Changed to Paid!!! ");
        }
    }

    //public async Task<bool> Handle(SetPaidOrderStatusCommand request, CancellationToken cancellationToken)
    //{
    //    await _mediator.Publish(new OrderStatusChangedToPaidEvent(order.Id));
    //    return true;
    //}
    public class SetPaidOrderStatusCommandResponse
    {
        public bool Finished { get; set; }
    }
}
#region conmment
// var order = await _order.FirstOrDefaultAsync(f => f.Id == context.Message.OrderId &&f.TenantId==context.Message.TenantId);
// var collection = _readDbContext.OrderCollection;
// if (order == null)
// {
//     _logger.LogError($"order {context.Message.OrderId}'s Status Update to Paid Failed!!!!");
//     await context.RespondAsync(new SetPaidOrderStatusCommandResponse { Finished = false });
// }
// else
// {
// try
// {
// //==================readDb
// var filter = Builders<OrderCollection>.Filter.Eq(e => e.OrderId, context.Message.OrderId);
// var orderRead = await collection.Find(f => f.OrderId == context.Message.OrderId).FirstOrDefaultAsync();
// orderRead.OrderStatus = OrderStatus.Paid;
// await collection.ReplaceOneAsync(filter, orderRead);
// //==================writeDb
// order.OrderStatus = OrderStatus.Paid;
// _order.Update(order);
// await _uow.SaveChangesAsync();
// }
// catch (Exception e)
// {
//     _logger.LogError($"order {context.Message.OrderId}'s Status Update to Paid Failed!!!!");
//     _logger.LogDebug($"order {context.Message.OrderId}'s Status Change Failed with error {e.Message} : {e}");
//     await context.RespondAsync(new SetPaidOrderStatusCommandResponse { Finished = false });

// } F
#endregion
