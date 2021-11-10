using Cloudware.Microservice.Ordering.Application.Command;
using Cloudware.Utilities.Contract.Ordering;
using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Model
{
    public class Order
    {
        public long Id { get; set; }
        public string TenantId { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public DateTimeOffset? DeliveryDate { get; set; }
        public DateTimeOffset? PaidDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal ShippingPrice { get; set; }
        public string ShippingPriceId { get; set; }
        public string CorrelationId { get; set; }
        public double? Discount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentType PaymentType { get; set; }
        public long WalletAccountId { get; set; }
        public string AddressId { get; set; }
        //shipping 
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

    }

    public enum OrderStatus
    {
        [Display(Name = "درانتظار پرداخت", Order = 0, AutoGenerateField = true)]
        WaitingForPayment,
        [Display(Name = "در حال پردازش", Order = 11, AutoGenerateField = false)]
        Processing,
        [Display(Name = "پرداخت با خطا مواجه شد", Order = 12, AutoGenerateField = false)]
        PaymentFailed,
        [Display(Name = "پرداخت شده", Order = 1, AutoGenerateField = true)]
        Paid,
        [Display(Name = "تایید انبار", Order = 13, AutoGenerateField = false)]
        StockConfirmed,
        [Display(Name = "در انبار موجود نیست", Order = 14, AutoGenerateField = false)]
        StockNotConfirmed,
        // [Display(Name = "ارسال شده", Order = 4, AutoGenerateField = true)]
        // Shipped,
        [Display(Name = "تایید شده", Order = 3, AutoGenerateField = true)]
        Submitted,
        [Display(Name = "در حال اعتبار سنجی", Order = 2, AutoGenerateField = true)]
        AwaitingValidation,
        [Display(Name = "دریافت شده", Order = 5, AutoGenerateField = true)]
        Delivered,
        [Display(Name = "لغو شده", Order = 10, AutoGenerateField = false)]
        Canceled,
        [Display(Name = "init", Order = 10, AutoGenerateField = false)]
        init,
        [Display(Name = "shippingPriceConfirmed", Order = 10, AutoGenerateField = false)]
        shippingPriceConfirmed,
        [Display(Name = "addressConfirmed", Order = 10, AutoGenerateField = false)]
        addressConfirmed,
        [Display(Name = "dataReceivedFromProduct", Order = 10, AutoGenerateField = false)]
        dataReceivedFromProduct,
        [Display(Name = "درحال اماده سازی", Order = 10, AutoGenerateField = true)]
        preparing,
        [Display(Name = "درحال ارسال", Order = 10, AutoGenerateField = true)]
        sending
    }
}
