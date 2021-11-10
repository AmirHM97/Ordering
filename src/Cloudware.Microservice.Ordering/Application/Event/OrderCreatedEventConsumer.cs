using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Infrastructure.Extension;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Microservice.Ordering.Model.ReadDb;
using Cloudware.Utilities.BusTools;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Ordering;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using src.Ordering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Event
{

    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>, IMediatorConsumerType
    {
        private readonly IOrderingReadDbContext _readContext;
        // private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IPublishEndpointInSingletonService _publishEndpointInSingletonService;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;
        public OrderCreatedEventConsumer(ILogger<OrderCreatedEventConsumer> logger, IOrderingReadDbContext readContext, IPublishEndpointInSingletonService publishEndpointInSingletonService)
        {
            _logger = logger;
            _readContext = readContext;
            _publishEndpointInSingletonService = publishEndpointInSingletonService;
        }
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            try
            {
                var collection = _readContext.OrderCollection;

                var orderItems = context.Message.OrderCreatedItems.Select(s => new OrderItemReadDb
                {
                    OrderItemId = s.Id,
                    Discount = s.Discount,
                    // OldUnitPrice = s.OldUnitPrice,
                    PictureUrl = s.PictureUrl,
                    //  PostedBy = s.PostedBy,
                    ProductId = s.ProductId,
                    ProductName = s.ProductName,
                    CategoryId = s.CategoryId,
                    CategoryName = s.CategoryName,
                    Properties = s.Properties.Select(p => new Property
                    {
                        Name=p.Name,
                        PropertyItemsDto=p.PropertyItemsDto.Select(sp=>new PropertyItem{
                            Id=sp.Id,
                            Name=sp.Name,
                            Value=sp.Value
                        }).ToList(),
                        PropertyType=p.PropertyType,
                        PropertyTypeName=p.PropertyType.ToString(),
                        PropertyViewType=p.PropertyViewType,
                        PropertyViewTypeName=p.PropertyViewType.ToString()
                    }).ToList(),
                    Quantity = s.RequestedCount,
                    SellerName = s.SellerName,
                    SellerAddress = s.SellerAddress,
                    SellerMobile = s.SellerMobile,
                    StockId = s.StockItemDto.Id,
                    UnitPrice = s.StockItemDto.Price,
                    SellerUserId = s.SellerUserId,
                    StockItemDto = OrderingExtensions.MapStockItemDtoToStockItemDtoReadDb(s.StockItemDto)
                }).ToList();

                var order = new OrderCollection
                {
                    //todo add cityId and provinceId
                    CorrelationId = context.Message.CorrelationId,
                    TenantId = context.Message.TenantId,
                    UserId = context.Message.UserId,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
                    Discount = context.Message.Discount,
                    AddressId = context.Message.AddressId,
                    UserFirstName = context.Message.FirstName,
                    UserLastName = context.Message.LastName,
                    BasketId = context.Message.BasketId,
                    UserPhone = context.Message.UserPhone,
                    // ProvinceName = context.Message.Province,
                    // CityName = context.Message.City,
                    // LandlinePhoneNumber = context.Message.LandlinePhoneNumber,
                    // MobilePhoneNumber = context.Message.MobilePhoneNumber,
                    // PostalAddress = context.Message.PostalAddress,
                    // PostalCode = context.Message.PostalCode,
                    ShippingPriceId = context.Message.ShippingPriceId,
                    // ShippingPrice = context.Message.ShippingPrice,
                    TotalPrice = context.Message.TotalPrice,
                    OrderId = context.Message.OrderId,
                    PaymentType = Utilities.Contract.Ordering.PaymentType.Wallet,
                    PaymentTypeName=Utilities.Contract.Ordering.PaymentType.Wallet.ToString(),
                    OrderStatus = OrderStatus.WaitingForPayment,
                    OrderStatusName= OrderStatus.WaitingForPayment.ToString(),
                    OrderItems = orderItems,

                };
                await collection.InsertOneAsync(order);
                _logger.LogInformation($"order {context.Message.OrderId} added to readDb");

                await _publishEndpointInSingletonService.Publish(new OrderCreationStartedEvent(order.OrderId, order.TenantId, order.AddressId, order.ShippingPriceId, context.Message.OrderCreatedItems.Select(s => new BasketItem
                {
                    ProductId = s.ProductId,
                    StockItemDto = s.StockItemDto,
                    Properties = s.Properties,
                    RequestedCount = s.RequestedCount
                }).ToList(), order.TotalPrice));
               
                // await _publishEndpointInSingletonService.Publish(new OrderCreatedCompletelyEvent(context.Message.BasketId, context.Message.UserId, context.Message.OrderId, context.Message.TotalPrice));


            }
            catch (Exception e)
            {
                //publish some event
                _logger.LogError($"order {context.Message.OrderId} add to readDb failed!!!");
                _logger.LogDebug($"order {context.Message.OrderId} add to readDb failed!!! {e.Message} : {e}");

                throw;
            }
        }

    }
}

// if (order.PaymentType == Utilities.Contract.Ordering.PaymentType.Wallet)
// {
//     var orderCreatedCompletelyEvent = new OrderCreatedCompletelyForWalletEvent
//     {
//         BasketId = context.Message.BasketId,
//         OrderId = context.Message.OrderId,
//         TotalPrice = context.Message.TotalPrice,
//         UserId = context.Message.UserId
//     };
//     using (var scope = _serviceScopeFactory.CreateScope())
//     {
//         var publisher = scope.ServiceProvider.GetService<IPublishEndpoint>();
//         await publisher.Publish(orderCreatedCompletelyEvent);
//     }
// }
// else if (order.PaymentType == Utilities.Contract.Ordering.PaymentType.paymentGateway)
// {
//     var orderCreatedCompletelyEvent = new OrderCreatedCompletelyForPaymentGatewayEvent
//     {
//         BasketId = context.Message.BasketId,
//         OrderId = context.Message.OrderId,
//         TotalPrice = context.Message.TotalPrice,
//         UserId = context.Message.UserId
//     };
//     using (var scope = _serviceScopeFactory.CreateScope())
//     {
//         var publisher = scope.ServiceProvider.GetService<IPublishEndpoint>();
//         await publisher.Publish(orderCreatedCompletelyEvent);
//     }
// }