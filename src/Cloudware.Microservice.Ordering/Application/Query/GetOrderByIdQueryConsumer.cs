using Cloudware.Microservice.Ordering.DTO;
using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Microservice.Ordering.Model.ReadDb;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Cloudware.Microservice.Ordering.Application.Query.GetOrderDetailStatusQueryConsumer;

namespace Cloudware.Microservice.Ordering.Application.Query
{
    public class GetOrderByIdQueryConsumer : IConsumer<GetOrderByIdQuery>, IMediatorConsumerType
    {
        private readonly IOrderingReadDbContext _readDbContext;
        private readonly IMediator _mediator;
        public GetOrderByIdQueryConsumer(IMediator mediator, IOrderingReadDbContext readDbContext)
        {
            _mediator = mediator;
            _readDbContext = readDbContext;
        }
        public async Task Consume(ConsumeContext<GetOrderByIdQuery> context)
        {
            var collection = _readDbContext.OrderCollection;
            var order = await collection.Find(f => f.OrderId == context.Message.OrderId).FirstOrDefaultAsync();
            var orderDto = new OrderDetailDto
            {
                OrderId = order.OrderId,
                CreatedDate = order.CreatedDate,
                DeliveryDate = order.DeliveryDate,
                Discount = order.Discount,
                OrderStatus = order.OrderStatus,
                PaymentType = order.PaymentType,
                PostalAddress = order.PostalAddress,
                TotalPrice = order.TotalPrice,
                ShippingPrice = order.ShippingPrice,
                UserFirstName = order.UserFirstName,
                UserLastName = order.UserLastName,
                UserPhone = order.UserPhone,
                OrderItems = order.OrderItems,
                ReceiverFirstName=order.ReceiverFirstName,
                ReceiverLastName=order.ReceiverLastName
            };
            if (context.Message.StatusNeeded)
            {
                var statReq = _mediator.CreateRequestClient<GetOrderDetailStatusQuery>();
                var statData = await statReq.GetResponse<GetOrderDetailStatusQueryResponse>(new GetOrderDetailStatusQuery(order.OrderId));
                orderDto.OrderDetailStatusItems = statData.Message.OrderDetailStatusItems;
            }
            await context.RespondAsync(new GetOrderByIdQueryResponse(orderDto));


        }
        public class GetOrderByIdQueryResponse
        {
            public OrderDetailDto OrderDetail { get; set; }

            public GetOrderByIdQueryResponse(OrderDetailDto orderDetail)
            {
                OrderDetail = orderDetail;
            }
        }
    }
}
