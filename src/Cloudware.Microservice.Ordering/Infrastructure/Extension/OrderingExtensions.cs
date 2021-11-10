using Cloudware.Microservice.Ordering.Model;
using Cloudware.Microservice.Ordering.Model.ReadDb;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Contract.PaymentGatway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Infrastructure.Extension
{
    public static class OrderingExtensions
    {

        // public static List<(OrderStatus orderStatus,List<OrderStatus>)>
        public static StockItemDtoReadDb MapStockItemDtoToStockItemDtoReadDb(StockItemDto stockItemDto)
        {
            var result = new StockItemDtoReadDb
            {
                Id = stockItemDto.Id,
                Count = stockItemDto.Count,
                Price = stockItemDto.Price,


                StockPropertyItems = stockItemDto.StockPropertyItems.Select(s => new Model.ReadDb.StockPropertyItem
                {
                    PropertyId = s.PropertyId,
                    PropertyType = s.PropertyType,
                    PropertyTypeName = s.PropertyType.ToString()
                }).ToList()

            };
            return result;
        }

        public static Device GetDevice(string status)
        {
            var result = status.ToLower() switch
            {
                "ios" => Device.ios,
                "android" => Device.android,
                "pwa" => Device.pwa,

                _ => Device.pwa,
            };
            return result;
        }
    }
}
