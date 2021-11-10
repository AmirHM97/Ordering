using Cloudware.Microservice.Ordering.Application.Event;
using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.BusTools;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Ordering;
using MassTransit;
using MassTransit.Mediator;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Command
{
    public class CreateOrderCommandConsumer : IConsumer<CreateOrderCommand>, IMediatorConsumerType
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Order> _orders;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateOrderCommandConsumer> _logger;

        public CreateOrderCommandConsumer(IUnitOfWork uow, ILogger<CreateOrderCommandConsumer> logger, IMediator mediator)
        {
            _uow = uow;
            _orders = _uow.Set<Order>();
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<CreateOrderCommand> context)
        {
            var orderItems = context.Message.BasketCompletedEvent.BasketItems
                  .Select(s =>
                  new OrderItem
                  {
                      ProductId = s.ProductId,
                      StockId = s.StockItemDto.Id,
                      Price = s.StockItemDto.Price,
                      Discount = s.Discount,
                      Quantity = s.RequestedCount,
                      SellerUserId = s.SellerUserId
                  }).ToList();
            // decimal totalPrice = 0;
            // foreach (var item in orderItems)
            // {
            //     totalPrice += item.Quantity * item.Price;
            // }
            var order = new Order
            {
                CorrelationId = context.Message.BasketCompletedEvent.CorrelationId,
                AddressId = context.Message.BasketCompletedEvent.AddressId,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow,
                ShippingPriceId = context.Message.BasketCompletedEvent.ShippingPriceId,
                OrderStatus = OrderStatus.init,
                TenantId = context.Message.BasketCompletedEvent.TenantId,
                UserId = context.Message.BasketCompletedEvent.UserId,
                PaymentType =  Cloudware.Utilities.Contract.Ordering.PaymentType.Wallet,
                OrderItems = orderItems,
                TotalPrice = context.Message.BasketCompletedEvent.TotalPrice,
            };
            await _orders.AddAsync(order);
            await _uow.SaveChangesAsync();
            
            var orderCreatedEventItems = orderItems
                   .Select((s, a) => new Event.OrderCreatedEventItem
                   {
                       Id = s.Id,
                       Discount = s.Discount.Value,
                       PictureUrl = context.Message.BasketCompletedEvent.BasketItems[a].PictureUrl,
                       ProductId = s.ProductId,
                       ProductName = context.Message.BasketCompletedEvent.BasketItems[a].ProductName,
                       RequestedCount = s.Quantity,
                       CategoryId=context.Message.BasketCompletedEvent.BasketItems[a].CategoryId,
                       CategoryName=context.Message.BasketCompletedEvent.BasketItems[a].CategoryName,
                       SellerName = context.Message.BasketCompletedEvent.BasketItems[a].SellerName,
                       Properties = context.Message.BasketCompletedEvent.BasketItems[a].Properties,
                       StockItemDto = context.Message.BasketCompletedEvent.BasketItems[a].StockItemDto,
                       SellerUserId = context.Message.BasketCompletedEvent.BasketItems[a].SellerUserId
                   }).ToList();

            await _mediator.Publish(new Event.OrderCreatedEvent(context.Message.BasketCompletedEvent.UserFirstName, context.Message.BasketCompletedEvent.UserLastName, order.Id, context.Message.BasketCompletedEvent.UserId,context.Message.BasketCompletedEvent.UserPhone, context.Message.BasketCompletedEvent.AddressId, context.Message.BasketCompletedEvent.BasketId, context.Message.BasketCompletedEvent.ShippingPriceId, context.Message.BasketCompletedEvent.TotalPrice, context.Message.BasketCompletedEvent.Discount, PaymentType.Wallet, orderCreatedEventItems, context.Message.BasketCompletedEvent.TenantId, context.Message.BasketCompletedEvent.CorrelationId));
        }

        public class CreateOrderCommandResponse
        {
            public long OrderId { get; set; }

            public CreateOrderCommandResponse(long orderId)
            {
                OrderId = orderId;
            }
        }
    }
}
#region old

//  if (context.Message.ShippingPriceConfirmedEvent.BasketData.BasketItems.Count == 0)
// {
//     throw new AppException(5059, HttpStatusCode.BadRequest, "basket item is empty");
// }
// try
// {
//     var basketData = context.Message.ShippingPriceConfirmedEvent.BasketData;

//     var orderItems = context.Message.ShippingPriceConfirmedEvent.BasketData.BasketItems
//         .Select(s =>
//         new OrderItem
//         {
//             ProductId = s.ProductId,
//             StockId = s.StockItemDto.Id,
//             Price = s.StockItemDto.Price,
//             Discount = s.Discount,
//             Quantity = s.RequestedCount,
//             SellerUserId = s.SellerUserId
//         }).ToList();
//     decimal totalPrice = 0;
//     foreach (var item in orderItems)
//     {
//         totalPrice = totalPrice + (item.Quantity * item.Price);
//     }
//     var order = new Order
//     {
//         TenantId=context.Message.ShippingPriceConfirmedEvent.BasketData.TenantId,
//         CorrelationId = context.CorrelationId.ToString(),
//         UserId = basketData.UserId,
//         AddressId = basketData.AddressId,
//         CreatedDate = DateTime.UtcNow,
//         LastUpdatedDate = DateTime.UtcNow,
//         Discount = basketData.Discount,
//         OrderStatus = OrderStatus.WaitingForPayment,
//         PaymentType = context.Message.PaymentType,
//         ShippingPrice = basketData.ShippingPrice,
//         ShippingPriceId = basketData.ShippingPriceId,
//         TotalPrice = totalPrice,
//         OrderItems = orderItems
//     };
//     await _orders.AddAsync(order);
//     await _uow.SaveChangesAsync();
//     await context.RespondAsync(new CreateOrderCommandResponse(order.Id));
//     _logger.LogDebug($"order : {order} added to writeDb");

//     var shippingData = context.Message.ShippingPriceConfirmedEvent;

//     //========================publish event============================
//     var orderCreatedEventItems=orderItems
//         .Select((s, a) => new OrderCreatedEventItem
//         {
//             Id = s.Id,
//             Discount = s.Discount.Value,
//             PictureUrl = basketData.BasketItems[a].PictureUrl,
//             ProductId = s.ProductId,
//             ProductName = basketData.BasketItems[a].ProductName,
//             RequestedCount = s.Quantity,
//             SellerName = basketData.BasketItems[a].SellerName,
//             Properties = basketData.BasketItems[a].Properties,
//             StockItemDto = basketData.BasketItems[a].StockItemDto,
//             SellerUserId = basketData.BasketItems[a].SellerUserId
//         }).ToList();
//     await _mediator.Publish(new OrderCreatedEvent(shippingData.FirstName, shippingData.LastName,
//         shippingData.City, shippingData.Province, shippingData.LandlinePhoneNumber,
//         shippingData.MobilePhoneNumber, shippingData.PostalCode, shippingData.PostalAddress,
//         shippingData.Latitude, shippingData.Longitude, order.Id, basketData.UserId,
//         basketData.AddressId, basketData.BasketId, basketData.ShippingPriceId, totalPrice,
//         basketData.ShippingPrice, basketData.Discount, context.Message.PaymentType,orderCreatedEventItems,shippingData.BasketData.TenantId,context.Message.CorrelationId.ToString()));
// }
// catch (Exception e)
// {
//     _logger.LogDebug($"order : {context.Message} insertion to writeDb failed!");

//     throw;
// }
#endregion