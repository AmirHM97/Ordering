using System;
using System.Collections.Generic;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Microservice.Ordering.Model.ReadDb;
using Cloudware.Utilities.Contract.Basket;
using static Cloudware.Microservice.Ordering.Application.Query.GetOrderDetailStatusQueryConsumer;

namespace Cloudware.Microservice.Ordering.DTO
{
    public class OrderDetailDto
    {
        public long OrderId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserPhone { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset DeliveryDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal ShippingPrice { get; set; }
        public double? Discount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string PostalAddress { get; set; }
         public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public List<GetOrderDetailStatusQueryResponseItem> OrderDetailStatusItems { get; set; }

        public Utilities.Contract.Ordering.PaymentType PaymentType { get; set; }
        public List<OrderItemReadDb> OrderItems { get; set; }
    }
    // public class OrderItemDto
    // {
    //     public long OrderItemId { get; set; }
    //     public long ProductId { get; set; }
    //     public string ProductName { get; set; }
    //     public long StockId { get; set; }
    //     public long Quantity { get; set; }
    //     public string PictureUrl { get; set; }
    //     public decimal UnitPrice { get; set; }
    //       public List<OrderDetailPropertyDto> Properties { get; set; }
    //     public StockItemDto StockItem { get; set; }
    // }
    // public class StockItemDto
    // {
    //     public long Id { get; set; }
    //     public long Count { get; set; }
    //     public decimal Price { get; set; }
    //     public List<StockPropertyItemDto> StockPropertyItems { get; set; }
    // }
    // public class StockPropertyItemDto
    // {
    //     public long PropertyId { get; set; }
    //     public PropertyType PropertyType { get; set; }
    // }
    // public class OrderDetailPropertyDto
    // {
    //     public string Name { get; set; }
    //     public PropertyType PropertyType { get; set; }
    //     public PropertyViewType PropertyViewType { get; set; }
    //     public List<OrderDetailPropertyItemDto> PropertyItemsDto { get; set; }

    // }
    // public class OrderDetailPropertyItemDto
    // {
    //     public long Id { get; set; }

    //     public string Name { get; set; }
    //     public string? Value { get; set; }
    // }

}