using Cloudware.Microservice.Ordering.Application.Event;
using Cloudware.Utilities.Contract.Basket;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Model.ReadDb
{
    public class OrderItemReadDb
    {
        public long OrderItemId { get; set; }
        public long ProductId { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public long StockId { get; set; }
        public string SellerName { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal UnitPrice { get; set; }
        public double Discount { get; set; }
        public string PostedBy { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal OldUnitPrice { get; set; }

        public long Quantity { get; set; }
        public string PictureUrl { get; set; }

        public string SellerUserId { get; set; }
        public string SellerMobile { get; set; }
        public string SellerAddress { get; set; }
        public string SellerCity { get; set; }
        public long SellerCityId { get; set; }
        public string SellerProvince { get; set; }
        public long SellerProvinceId { get; set; }

        public List<Property> Properties { get; set; }
        public StockItemDtoReadDb StockItemDto { get; set; }
    }
    public class Property
    {
        public string Name { get; set; }
        public PropertyType PropertyType { get; set; }
        public string PropertyTypeName { get; set; }
        public PropertyViewType PropertyViewType { get; set; }
        public string PropertyViewTypeName { get; set; }
        public List<PropertyItem> PropertyItemsDto { get; set; }
    }

    public class PropertyItem
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string? Value { get; set; }
    }
    public class StockItemDtoReadDb
    {
        public long Id { get; set; }
        public long Count { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Price { get; set; }
        public List<StockPropertyItem> StockPropertyItems { get; set; }

    }
    public class StockPropertyItem
    {
        public long PropertyId { get; set; }
        public PropertyType PropertyType { get; set; }
        public string PropertyTypeName { get; set; }
    }
}
