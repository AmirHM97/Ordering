using Cloudware.Microservice.Ordering.Application.Command;
using Cloudware.Microservice.Ordering.Application.Event;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Contract.Ordering;
using Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Model.ReadDb
{
    public class OrderCollection
    {
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public long OrderId { get; set; }
        public string TenantId { get; set; }
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserPhone { get; set; }

        public string CorrelationId { get; set; }
        public string OrderStatusMessage { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public DateTimeOffset DeliveryDate { get; set; }
        public DateTimeOffset PaidDate { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalPrice { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal ShippingPrice { get; set; }
        public double? Discount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string OrderStatusName { get; set; }
        public Utilities.Contract.Ordering.PaymentType PaymentType { get; set; }
        public long? WalletAccountId { get; set; }=0;
        public string? WalletAccountName { get; set; }="";
        
        public string PaymentTypeName { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string LandlinePhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string PostalCode { get; set; }
        public string PostalAddress { get; set; }
        public string AddressId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public List<OrderItemReadDb> OrderItems { get; set; }
        public string ShippingPriceId { get; set; }
        public bool IsDelete { get; set; }=false;
        public List<OrderStatus> OrderStatusLogs { get; set; }=new();
        public int Version { get; set; }=0;
        public string BasketId { get; set; }="";
        
    }

     public class OrderCollectionTest
    {
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public long OrderId { get; set; }
        public string TenantId { get; set; }
        public string UserId { get; set; }
      
    }

}
