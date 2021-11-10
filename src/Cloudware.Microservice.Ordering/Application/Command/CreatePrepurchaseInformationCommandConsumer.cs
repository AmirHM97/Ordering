using Cloudware.Microservice.Ordering.Application.Query;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Contract.Notification;
using FizzWare.NBuilder;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Cloudware.Microservice.Ordering.Application.Query.GetOrderByIdQueryConsumer;

namespace Cloudware.Microservice.Ordering.Application.Command
{
    public class CreatePrepurchaseInformationCommandConsumer : IConsumer<CreatePrepurchaseInformationCommand>, IMediatorConsumerType
    {
        private readonly IRequestClient<GetOrderByIdQuery> _getOrderByIdQuery;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public CreatePrepurchaseInformationCommandConsumer(IRequestClient<GetOrderByIdQuery> getOrderByIdQuery,
            IServiceScopeFactory serviceScopeFactory)
        {
            _getOrderByIdQuery = getOrderByIdQuery;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task Consume(ConsumeContext<CreatePrepurchaseInformationCommand> context)
        {
            var order = await _getOrderByIdQuery.GetResponse<GetOrderByIdQueryResponse>(new GetOrderByIdQuery(context.Message.Id,false));
            var guid = Guid.NewGuid();
            var prePurchaseInformationEvent = new PrePurchaseInformationEvent
            {
                TotalPrice = order.Message.OrderDetail.TotalPrice,
                PaymentType = PaymentType.Buy,
                UserId = context.Message.UserId,
                TenantId = context.Message.TenantId,
                OrderId = context.Message.Id,
                Token = guid.ToString(),
                FirstName = context.Message.FirstName,
                LastName = context.Message.LastName,
                DeviceType=context.Message.Device
            };

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<IPublishEndpoint>();
                await db.Publish(prePurchaseInformationEvent);
            }
            var generator = new RandomGenerator();
            var factorNumber = generator.Next(11111, 9999999);
            await context.RespondAsync(new CreatePrepurchaseInformationCommandResponse(guid.ToString(), order.Message.OrderDetail.OrderId,(int) order.Message.OrderDetail.TotalPrice, order.Message.OrderDetail.UserFirstName+order.Message.OrderDetail.ReceiverLastName, factorNumber));
        }

        public class CreatePrepurchaseInformationCommandResponse
        {
            public CreatePrepurchaseInformationCommandResponse(string token, long orderId, decimal totalPrice, string fullname, int factorNumber)
            {
                Token = token;
                OrderId = orderId;
                TotalPrice = totalPrice;
                Fullname = fullname;
                FactorNumber = factorNumber;
            }

            public string Token { get; set; }
            public long OrderId { get; set; }
            public decimal TotalPrice { get; set; }
            public string Fullname { get; set; }
            public int FactorNumber { get; set; }
        }
    }
}
