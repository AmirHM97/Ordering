using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudware.Microservice.Ordering.Application.Command;
using Cloudware.Microservice.Ordering.Application.Command.Status;
using Cloudware.Microservice.Ordering.DTO;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.BusTools;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Ordering;
using Cloudware.Utilities.Contract.Shipping;
using MassTransit;
using MassTransit.Mediator;

namespace Cloudware.Microservice.Ordering.Application.Event.Shipping
{
    public class OrderCreationAddressConfirmedEventConsumer : IConsumer<OrderCreationAddressConfirmedEvent>, IBusConsumerType
    {
        private readonly IOrderService _orderService;
        private readonly IPublishEndpointInSingletonService _publishEndpoint;
        private readonly IMediator _mediator;

        public OrderCreationAddressConfirmedEventConsumer(IOrderService orderService, IMediator mediator)
        {
            _orderService = orderService;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<OrderCreationAddressConfirmedEvent> context)
        {
            await _orderService.UpdateAddress(new UpdateAddressDto
            {
                City = context.Message.City,
                CityId = context.Message.CityId,
                FirstName = context.Message.FirstName,
                LandlinePhoneNumber = context.Message.LandlinePhoneNumber,
                LastName = context.Message.LastName,
                Latitude = context.Message.Latitude,
                Longitude = context.Message.Longitude,
                MobilePhoneNumber = context.Message.MobilePhoneNumber,
                OrderId = context.Message.OrderId,
                PostalAddress = context.Message.PostalAddress,
                PostalCode = context.Message.PostalCode,
                Province = context.Message.Province,
                ProvinceId = context.Message.ProvinceId
            });
            await _orderService.UpdateOneStatusAsync(context.Message.TenantId,context.Message.OrderId, OrderStatus.addressConfirmed);
            var statusList = new List<OrderStatus>{
                OrderStatus.StockConfirmed,
                OrderStatus.shippingPriceConfirmed
            };
            var res = await _orderService.CheckExistingStatus(context.Message.OrderId, statusList);
            if (res)
            {
            
                
                await _mediator.Publish(new SetWaitingForPaymentOrderStatusCommand(context.Message.OrderId,context.Message.TenantId));
                await _mediator.Publish(new PublishOrderCreatedCompletelyEventCommand(context.Message.OrderId, context.Message.TenantId));
            }
        }
    }
}