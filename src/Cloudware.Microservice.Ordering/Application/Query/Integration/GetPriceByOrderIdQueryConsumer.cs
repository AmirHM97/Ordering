using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Ordering;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Query.Integration
{
    public class GetPriceByOrderIdQueryConsumer : IConsumer<GetPriceByOrderIdQuery>, IBusConsumerType
    {
        private readonly DbSet<Order> _orders;
        private readonly IUnitOfWork _uow;

        public GetPriceByOrderIdQueryConsumer(IUnitOfWork uow)
        {
            _uow = uow;
            _orders = _uow.Set<Order>();
        }

        public async Task Consume(ConsumeContext<GetPriceByOrderIdQuery> context)
        {
            var price = await _orders.Where(w => w.Id == Int32.Parse(context.Message.OrderId)).Select(s => s.TotalPrice).FirstOrDefaultAsync();
            await context.RespondAsync(new GetPriceByOrderIdQueryResponse {Amount=price});
        }
    }
}
