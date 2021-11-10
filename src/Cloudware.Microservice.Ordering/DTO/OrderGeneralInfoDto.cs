using System;
using System.Collections.Generic;
using Cloudware.Microservice.Ordering.Model;

namespace Cloudware.Microservice.Ordering.DTO
{
    public class OrderGeneralInfoDto
    {
        public long OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<string>OrderItemImageUrls { get; set; }
    }
}