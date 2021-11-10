using System;

namespace Cloudware.Microservice.Ordering.Model
{
    public class OrderItem
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public virtual Order Order { get; set; }
        public long ProductId { get; set; }
         public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public long StockId { get; set; }
        public long Quantity { get; set; }
        public decimal Price { get; set; }
        public double? Discount { get; set; }
        //ReadDb
        public decimal TotalPrice { get; set; }
        public string SellerUserId { get; set; }

    }
}
