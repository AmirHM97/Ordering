using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using MassTransit;
using Microsoft.OpenApi.Extensions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Query
{
    public class GetOrderDetailStatusQueryConsumer : IConsumer<GetOrderDetailStatusQuery>, IMediatorConsumerType
    {
        private readonly IOrderingReadDbContext _orderingReadDbContext;

        public GetOrderDetailStatusQueryConsumer(IOrderingReadDbContext orderingReadDbContext)
        {
            _orderingReadDbContext = orderingReadDbContext;
        }

        public async Task Consume(ConsumeContext<GetOrderDetailStatusQuery> context)
        {
            var responseItem = new List<GetOrderDetailStatusQueryResponseItem>();
            //  responseItem.Add(new GetOrderDetailStatusQueryResponseItem {Id=(int)OrderStatus.WaitingForPayment,Text="در انتظار پرداخت" });
            var order = await _orderingReadDbContext.OrderCollection.Find(f => f.OrderId == context.Message.OrderId).FirstOrDefaultAsync();
            long activeStatusOrder = -1;
            foreach (OrderStatus val in Enum.GetValues(typeof(OrderStatus)))
            {
                if (val.GetAttributeOfType<DisplayAttribute>().AutoGenerateField)
                {
                    bool isActive = false;
                    if (order.OrderStatus == val)
                    {
                        isActive = true;
                        activeStatusOrder = val.GetAttributeOfType<DisplayAttribute>().Order;
                    }
                    var isDone = activeStatusOrder != -1 && activeStatusOrder< val.GetAttributeOfType<DisplayAttribute>().Order ?false:true;
                    responseItem.Add(new GetOrderDetailStatusQueryResponseItem
                    {
                        Id = (int)val,
                        Text = val.GetAttributeOfType<DisplayAttribute>().Name,
                        Order = val.GetAttributeOfType<DisplayAttribute>().Order,
                        IsActive = isActive,
                        IsDone = isActive==true?false: isDone
                    });
                }
            }
            var activeStatus = responseItem.FirstOrDefault(f=>f.IsActive);
            //responseItem = responseItem.Select(s => new GetOrderDetailStatusQueryResponseItem {
            //    Id = s.Id,
            //    Text = s.Text,
            //    Order = s.Order,
            //    IsActive = s.IsActive,
            //    IsDone=activeStatus.Order<s.Order?false:true
            //}).ToList();
            await context.RespondAsync(new GetOrderDetailStatusQueryResponse(responseItem.OrderBy(o => o.Order).ToList()));
        }
        public class GetOrderDetailStatusQueryResponse
        {
            public List<GetOrderDetailStatusQueryResponseItem> OrderDetailStatusItems { get; set; }

            public GetOrderDetailStatusQueryResponse(List<GetOrderDetailStatusQueryResponseItem> orderDetailStatusItems)
            {
                OrderDetailStatusItems = orderDetailStatusItems;
            }
        }
        public class GetOrderDetailStatusQueryResponseItem
        {
            public long Id { get; set; }
            public string Text { get; set; }
            public long Order { get; set; }
            public bool IsActive{ get; set; }
            public bool IsDone{ get; set; }

        }
    }
}
