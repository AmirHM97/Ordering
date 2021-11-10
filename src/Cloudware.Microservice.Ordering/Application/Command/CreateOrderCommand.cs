using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Ordering;
using Cloudware.Utilities.Contract.Shipping;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Command
{
    public record CreateOrderCommand(BasketCompletedEvent BasketCompletedEvent);
    
        // public CreateOrderCommand(ShippingPriceConfirmedEvent shippingPriceConfirmedEvent, Utilities.Contract.Ordering.PaymentType paymentType, Guid correlationId)
        // {
        //     ShippingPriceConfirmedEvent = shippingPriceConfirmedEvent;
        //     PaymentType = paymentType;
        //     CorrelationId = correlationId;
        // }

        // public ShippingPriceConfirmedEvent ShippingPriceConfirmedEvent { get; set; }
        // public Utilities.Contract.Ordering.PaymentType PaymentType { get; set; }
        // public Guid CorrelationId { get; set; }




 
    /// <summary>
    /// 0 = kookbazWallet
    /// </summary>
   
}
