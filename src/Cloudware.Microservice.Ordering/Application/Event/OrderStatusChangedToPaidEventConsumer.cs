using Cloudware.Microservice.Ordering.Application.Command.Crm;
using Cloudware.Microservice.Ordering.Application.Query;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Utilities.BusTools;
using Cloudware.Utilities.Configure.Microservice.Services;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Ordering;
using Cloudware.Utilities.Contract.Shop.Notification;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Cloudware.Microservice.Ordering.Application.Query.GetOrderByIdQueryConsumer;

namespace Cloudware.Microservice.Ordering.Application.Event
{
    public class OrderStatusChangedToPaidEventConsumer : IConsumer<OrderStatusChangedToPaidEvent>, IMediatorConsumerType
    {

        private readonly ILogger<OrderStatusChangedToPaidEventConsumer> _logger;
        private readonly IUserInformationService _userInformationService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOrderService _orderService;
        private readonly IMediator _mediator;
        private readonly IRequestClient<GetOrderByIdQuery> _getOrderByIdQuery;
        public OrderStatusChangedToPaidEventConsumer(ILogger<OrderStatusChangedToPaidEventConsumer> logger,
            IUserInformationService userInformationService,
            IServiceScopeFactory serviceScopeFactory,
           IRequestClient<GetOrderByIdQuery> getOrderByIdQuery
, IOrderService orderService, IMediator mediator)
        {

            _logger = logger;
            _userInformationService = userInformationService;
            _serviceScopeFactory = serviceScopeFactory;
            _getOrderByIdQuery = getOrderByIdQuery;
            _orderService = orderService;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<OrderStatusChangedToPaidEvent> context)
        {
            #region MyRegion
            //List<string> AuthorList = new List<string>();
            //var s = new List<Param>();
            //s.Add(new Param { Parameter = "key", ParameterValue = context.Message.OrderId.ToString() });
            //AuthorList.Add("09122591909");
            //var sms = new SmsParametr();
            //var l = sms.Param = s;
            //var notificationSendEvent = new NotificationSendEvent
            //{
            //    EventName = "order",
            //    //NotificationType = NotificationType.kaveNegar,
            //    // ParameterValue = "121212",
            //    TenantId = "17fcb4fa-3713-4007-ba97-b0f5f84027bf",
            //    SendTo = AuthorList,
            //    SmsParametr = sms
            //};  //if (context.Message.TenantId== "17fcb4fa-3713-4007-ba97-b0f5f84027bf")
            //{
            //    //var notificationSendEvent = new NotificationSendEvent("17fcb4fa-3713-4007-ba97-b0f5f84027bf", "order", SendType.sms)
            //    //{
            //    //    Parameters = new List<(string Key, string Value)>() { new("key", context.Message.OrderId.ToString()) },
            //    //    SendTo = new List<string> { "09193088378" },
            //    //    EventName = "order",
            //    //    TenantId = "17fcb4fa-3713-4007-ba97-b0f5f84027bf",

            //    //};
            //    var purchaseCompletedEvent = new PurchaseCompletedEvent("order", new List<(string Key, string Value)>() { new("key", context.Message.OrderId.ToString()) }, context.Message.TenantId);
            //    using (var scope = _serviceScopeFactory.CreateScope())
            //    {
            //        var db = scope.ServiceProvider.GetService<IPublishEndpoint>();
            //        await db.Publish(purchaseCompletedEvent);
            //    }
            //}
            #endregion
            await _mediator.Publish(new PublishCrmOrderCreatedEvent(context.Message.TenantId, context.Message.OrderId));
            await _orderService.UpdateOrderStatus(context.Message.TenantId, context.Message.OrderId, Model.OrderStatus.Processing);
            var order = await _getOrderByIdQuery.GetResponse<GetOrderByIdQueryResponse>(new GetOrderByIdQuery(context.Message.OrderId, false));
            var sellerList = order.Message.OrderDetail.OrderItems.Select(e => e.SellerUserId).ToList();
            var notificationSendEvent = new PurchaseCompletedEvent("order", new List<(string Key, string Value)>() { new("key", context.Message.OrderId.ToString()) }, context.Message.TenantId)
            {
                User = new List<string> { context.Message.UserId },
                SellerList = sellerList
            };

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<IPublishEndpoint>();
                await db.Publish(notificationSendEvent);
            }

            _logger.LogInformation($"OrderStatusChangedToPaidEvent Consumed!!!");
        }

        //public async Task Handle(OrderStatusChangedToPaidEvent notification, CancellationToken cancellationToken)
        //{

        //   // await _mediator.Publish(new )
        //    throw new NotImplementedException();
        //}
    }
}
