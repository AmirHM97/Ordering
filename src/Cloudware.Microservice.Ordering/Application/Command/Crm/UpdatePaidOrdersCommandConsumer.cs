using System.Threading.Tasks;
using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
using MongoDB.Driver;

namespace Cloudware.Microservice.Ordering.Application.Command.Crm
{
    public class UpdatePaidOrdersCommandConsumer : IConsumer<UpdatePaidOrdersCommand>, IMediatorConsumerType
    {
        private readonly IOrderingReadDbContext _orderingReadDbContext;
        private readonly IMediator _mediator;

        public UpdatePaidOrdersCommandConsumer(IOrderingReadDbContext orderingReadDbContext, IMediator mediator)
        {
            _orderingReadDbContext = orderingReadDbContext;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<UpdatePaidOrdersCommand> context)
        {
           var collection=_orderingReadDbContext.OrderCollection;
           var orders=await collection.Find(f=>f.OrderStatus==Model.OrderStatus.Processing).ToListAsync();
           foreach (var item in orders)
           {
               await _mediator.Publish(new PublishCrmOrderCreatedEvent(item.TenantId,item.OrderId));
           }
        }
    }
}