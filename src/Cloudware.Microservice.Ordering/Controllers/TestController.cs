using System.Threading.Tasks;
using Cloudware.Microservice.Ordering.Application.Command;
using Cloudware.Microservice.Ordering.Application.Command.Crm;
using Cloudware.Microservice.Ordering.Model.ReadDb;
using Cloudware.Utilities.Configure.Microservice;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Cloudware.Microservice.Ordering.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ClwControllerBase
    {
        private readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> test(long orderId)
        {
            
            #region commanet
            // await _publishEndpoint.Publish(new OrderCreatedCompletelyEvent("2", "3", orderId, 5000));


            //var notificationSendEvent = new NotificationSendEvent("17fcb4fa-3713-4007-ba97-b0f5f84027bf", "order", SendType.sms,PositionType.User)
            //{
            //    Parameters = new List<(string Key, string Value)>() { new("key", "55") },
            //    SendTo = new List<string> { "09193088378" },
            //    EventName = "order",
            //    TenantId = "17fcb4fa-3713-4007-ba97-b0f5f84027bf",
            //};

            //await _publishEndpoint.Publish(notificationSendEvent);


            // var notificationSendEvent = new PurchaseCompletedEvent("order", new List<(string Key, string Value)>() { new("key", "55") }, "17fcb4fa-3713-4007-ba97-b0f5f84027bf")
            // {
            //     User = new List<string> { "7d83e953-43fe-4c38-8f07-0cb9492557bf" },
            //     SendTo = new List<string> { "09193088378" }
            // };

            // await _publishEndpoint.Publish(notificationSendEvent);



            // var paymentDoneEvent = new PaymentDoneEvent
            // {
            //     Cash = 126000,
            //     UserId = "08070a8f-2aeb-4718-a843-509e52ea7665",
            //     Date = DateTime.UtcNow,
            //     OrderId = 554,
            //     PaymentType = Utilities.Contract.Basket.PaymentType.Buy,
            //     TransactionNumber = "122",
            //     Description = "",
            //     PaymentStatus = 1,
            //     TrackingCode = "9",
            //     Token = "4d84d481-e894-4c6f-9f4d-f811cf58fd71",
            //     FactorNumber = 8
            // };
            // await _publishEndpoint.Publish(paymentDoneEvent);
            // var orderStatusChangedToPaidEvent = new OrderStatusChangedToPaidEvent(1152, "b2cd6a53-146d-43b6-9dad-964a0b3c62ab", "364cbb18-b044-4339-b5c9-de772c2f011d");
            // await _mediator.Publish(orderStatusChangedToPaidEvent);
            // var d = "2021-08-01 08:40:35.8274153 +00:00";
            // var a = DateTimeOffset.Parse(d);
            // return Ok(a.ToLocalTime());
            // var list=new List<OrderStatus>{
            //     OrderStatus.StockConfirmed,
            //     OrderStatus.addressConfirmed
            // };
            // var res=await _orderService.CheckExistingStatus(1374,list);
            // var order = new OrderCollection();
            // await _orderingReadDbContext.OrderCollection.InsertOneAsync(order);
            #endregion
           await _mediator.Publish(new UpdatePaidOrdersCommand());
            return Ok();
        }
    }
}