using Cloudware.Microservice.Ordering.Application.Command;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Contract.Ordering;
using Cloudware.Utilities.Contract.Product;
using Cloudware.Utilities.Contract.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentType = Cloudware.Utilities.Contract.Ordering.PaymentType;
namespace Cloudware.Microservice.Ordering.Application.Event
{
    public record OrderCreatedEvent(string FirstName, string LastName, long OrderId, string UserId,string UserPhone,string AddressId, string BasketId, string ShippingPriceId, decimal TotalPrice, double Discount, PaymentType PaymentType, List<OrderCreatedEventItem> OrderCreatedItems, string TenantId, string CorrelationId);
    // public record OrderCreatedEvent(string FirstName, string LastName, string City, string Province,
    //         string LandlinePhoneNumber, string MobilePhoneNumber, string PostalCode, string PostalAddress,
    //         string Latitude, string Longitude, long OrderId, string UserId, string AddressId, string BasketId,
    //         string ShippingPriceId, decimal TotalPrice, decimal ShippingPrice, double Discount,
    //         PaymentType PaymentType, List<OrderCreatedEventItem> OrderCreatedItems, string TenantId,string CorrelationId);
    // {
    //     //=======================address Data==========================
    //     public string FirstName { get; set; }
    //     public string LastName { get; set; }
    //     public string City { get; set; }
    //     public string Province { get; set; }
    //     public string LandlinePhoneNumber { get; set; }
    //     public string MobilePhoneNumber { get; set; }
    //     public string PostalCode { get; set; }
    //     public string PostalAddress { get; set; }
    //     public string? Latitude { get; set; }
    //     public string? Longitude { get; set; }
    //     //=======================basket Data============================
    //     public long OrderId { get; set; }
    //     public string TenantId { get; set; }
    //     public string UserId { get; set; }
    //     public string AddressId { get; set; }
    //     public string BasketId { get; set; }
    //     public string ShippingPriceId { get; set; }
    //     public decimal TotalPrice { get; set; }
    //     public decimal ShippingPrice { get; set; }
    //     public double Discount { get; set; }
    //     public PaymentType PaymentType { get; set; }
    //     public List<OrderCreatedEventItem> OrderCreatedItems { get; set; }
    //     public OrderCreatedEvent(string firstName, string lastName, string city, string province,
    //         string landlinePhoneNumber, string mobilePhoneNumber, string postalCode, string postalAddress,
    //         string latitude, string longitude, long orderId, string userId, string addressId, string basketId,
    //         string shippingPriceId, decimal totalPrice, decimal shippingPrice, double discount,
    //         PaymentType paymentType, List<OrderCreatedEventItem> orderCreatedItems, string tenantId)
    //     {
    //         FirstName = firstName;
    //         LastName = lastName;
    //         City = city;
    //         Province = province;
    //         LandlinePhoneNumber = landlinePhoneNumber;
    //         MobilePhoneNumber = mobilePhoneNumber;
    //         PostalCode = postalCode;
    //         PostalAddress = postalAddress;
    //         Latitude = latitude;
    //         Longitude = longitude;
    //         OrderId = orderId;
    //         UserId = userId;
    //         AddressId = addressId;
    //         BasketId = basketId;
    //         ShippingPriceId = shippingPriceId;
    //         TotalPrice = totalPrice;
    //         ShippingPrice = shippingPrice;
    //         Discount = discount;
    //         PaymentType = paymentType;
    //         OrderCreatedItems = orderCreatedItems;
    //         TenantId = tenantId;
    //     }
    // }

    public class OrderCreatedEventItem
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string SellerName { get; set; }
        public string SellerUserId { get; set; }
        public string SellerAddress { get; set; }
        public string SellerMobile { get; set; }
        public long ProvinceId { get; set; }
        public string Province { get; set; }
        public long CityId { get; set; }
        public string City { get; set; }
        public double Discount { get; set; }
        public long RequestedCount { get; set; }
        public string PictureUrl { get; set; }
        public List<PropertyDto> Properties { get; set; }
        public StockItemDto StockItemDto { get; set; }
    }
}
