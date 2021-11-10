using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Utilities.BusTools;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Contract.Ordering.Crm;
using MassTransit;

namespace Cloudware.Microservice.Ordering.Application.Command.Crm
{
    public class PublishCrmOrderCreatedEventConsumer : IConsumer<PublishCrmOrderCreatedEvent>, IMediatorConsumerType
    {

        private readonly IOrderService _orderService;
        private readonly IPublishEndpointInSingletonService _publishEndpoint;

        public PublishCrmOrderCreatedEventConsumer(IOrderService orderService, IPublishEndpointInSingletonService publishEndpoint)
        {
            _orderService = orderService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<PublishCrmOrderCreatedEvent> context)
        {
        
            var order = await _orderService.GetOneReadAsync(context.Message.OrderId,context.Message.TenantId);
            var addressDto = new Utilities.Contract.Ordering.Crm.AddressDto(order.ReceiverFirstName, order.ReceiverLastName, order.AddressId, order.CityName, order.CityId, order.ProvinceName, order.ProvinceId, order.LandlinePhoneNumber, order.MobileNumber, order.PostalCode, order.PostalAddress, order.Latitude, order.Longitude);
            var orderItems = order.OrderItems.Select(s => new Utilities.Contract.Ordering.Crm.OrderCreatedEventItem
            {
                Id = s.OrderItemId,
                CategoryId = s.CategoryId,
                CategoryName = s.CategoryName,
                Discount = s.Discount,
                PictureUrl = s.PictureUrl,
                ProductId = s.ProductId,
                ProductName = s.ProductName,
                Properties = s.Properties.Select(p => new PropertyDto
                {
                    Name = p.Name,
                    PropertyItemsDto = p.PropertyItemsDto.Select(sp => new PropertyItemDto
                    {
                        Id = sp.Id,
                        Name = sp.Name,
                        Value = sp.Value
                    }).ToList(),
                    PropertyType = p.PropertyType,
                    PropertyViewType = p.PropertyViewType,

                }).ToList(),
                RequestedCount = s.Quantity,
                SellerInfo = new SellerInfo(s.SellerUserId, s.SellerName, s.SellerMobile, "", "", 0, "", 0, "", s.SellerAddress),
                StockItemDto = new Utilities.Contract.Basket.StockItemDto
                {
                    Count = s.StockItemDto.Count,
                    Id = s.StockItemDto.Id,
                    Price = s.StockItemDto.Price,
                    StockPropertyItems = s.StockItemDto.StockPropertyItems.Select(s => new StockPropertyItem { PropertyId = s.PropertyId, PropertyType = s.PropertyType }).ToList()
                }
            }).ToList();
            var crmOrderCreatedEvent = new OrderCreatedEvent(order.TenantId, order.OrderId, order.CreatedDate, order.UserId, order.UserFirstName, order.UserLastName, order.UserPhone, addressDto, orderItems, order.ShippingPrice, order.TotalPrice, order.Discount ?? 0, order.PaymentType);

            await _publishEndpoint.Publish(crmOrderCreatedEvent);
        }
    }
}