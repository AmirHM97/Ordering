using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cloudware.Microservice.Ordering.DTO;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Microservice.Ordering.Model.ReadDb;
using Cloudware.Utilities.Common.Exceptions;
using MassTransit.Saga;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Cloudware.Microservice.Ordering.Infrastructure.Services
{
    public interface IOrderService
    {
        Task<Order> GetOneAsync(long orderId, string tenantId);
        Task<OrderCollection> GetOneByCorrelationId(string correlationId);
        Task<OrderCollection> GetOneReadAsync(long orderId, string tenantId);
        Task<bool> CheckExistingStatus(long orderId, List<OrderStatus> orderStatuses);
        Task UpdateAddress(UpdateAddressDto updateAddressDto);
        Task UpdateOneStatusAsync(string TenantId, long orderId, OrderStatus orderStatus);
        Task<OrderCollection> UpdateOrderStatus(string tenantId, long orderId, OrderStatus orderStatus);
        Task UpdateOrderStatus(List<long> orderId, OrderStatus orderStatus);
        Task UpdateShippingPrice(string tenantId, long orderId, decimal shippingPrice);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderingReadDbContext _orderingReadDbContext;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Order> _orders;

        public OrderService(IOrderingReadDbContext orderingReadDbContext, IUnitOfWork uow)
        {
            _orderingReadDbContext = orderingReadDbContext;
            _uow = uow;
            _orders = _uow.Set<Order>();
        }
        public async Task UpdateAddress(UpdateAddressDto updateAddressDto)
        {

            var collection = _orderingReadDbContext.OrderCollection;
            UpdateResult updateResult;
            do
            {
                var loaded = await collection.Find(w => w.OrderId == updateAddressDto.OrderId).FirstOrDefaultAsync();
                var version = loaded.Version;

                loaded.Version++;
                var update = Builders<OrderCollection>.Update
                    .Set(p => p.ReceiverFirstName, updateAddressDto.FirstName)
                    .Set(p => p.ReceiverLastName, updateAddressDto.LastName)
                    .Set(p => p.CityName, updateAddressDto.City)
                    .Set(p => p.CityId, updateAddressDto.CityId)
                    .Set(p => p.ProvinceId, updateAddressDto.ProvinceId)
                    .Set(p => p.ProvinceName, updateAddressDto.Province)
                    .Set(p => p.LandlinePhoneNumber, updateAddressDto.LandlinePhoneNumber)
                    .Set(p => p.MobileNumber, updateAddressDto.MobilePhoneNumber)
                    .Set(p => p.PostalAddress, updateAddressDto.PostalAddress)
                    .Set(p => p.Latitude, updateAddressDto.Latitude)
                    .Set(p => p.Longitude, updateAddressDto.Longitude)
                    .Set(p => p.Version, loaded.Version);

                updateResult = await collection.UpdateOneAsync(f => f.OrderId == updateAddressDto.OrderId && f.Version == version, update, new UpdateOptions { IsUpsert = false });
            } while (updateResult.ModifiedCount != 1);



        }
        public async Task UpdateShippingPrice(string tenantId, long orderId, decimal shippingPrice)
        {
            var order = await _orders.FirstOrDefaultAsync(f => f.Id == orderId && f.TenantId == tenantId);
            var collection = _orderingReadDbContext.OrderCollection;
            if (order != null)
            {
                order.ShippingPrice = shippingPrice;
                _orders.Update(order);
                try
                {
                    await _uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync();
                    }
                    await UpdateShippingPrice(tenantId, orderId, shippingPrice);
                }
                UpdateResult updateResult;
                do
                {
                    var loaded = await collection.Find(w => w.OrderId == orderId && w.TenantId == tenantId).FirstOrDefaultAsync();
                    var version = loaded.Version;

                    loaded.Version++;
                    var update = Builders<OrderCollection>.Update
                        .Set(p => p.ShippingPrice, shippingPrice)
                        .Set(p => p.Version, loaded.Version);

                    updateResult = await collection.UpdateOneAsync(f => f.OrderId == orderId && f.Version == version, update, new UpdateOptions { IsUpsert = false });
                } while (updateResult.ModifiedCount != 1);

            }

        }
        public async Task<Order> GetOneAsync(long orderId, string tenantId)
        {
            var order = await _orders.FirstOrDefaultAsync(f => f.Id == orderId && f.TenantId == tenantId);
            return order;
        }
        public async Task<OrderCollection> GetOneReadAsync(long orderId, string tenantId)
        {
            var collection = _orderingReadDbContext.OrderCollection;
            var order = await collection.Find(f => f.OrderId == orderId && f.TenantId == tenantId).FirstOrDefaultAsync();
            return order;
        }
        public async Task<OrderCollection> UpdateOrderStatus(string tenantId, long orderId, OrderStatus orderStatus)
        {
            // var orderRead = await collection.Find(f => f.OrderId == orderId && f.TenantId == tenantId).FirstOrDefaultAsync();
            // var order = await _orders.FirstOrDefaultAsync(f => f.Id == orderId && f.TenantId == tenantId);
            // if (orderRead is null || order is null)
            // {
            //     throw new AppException(5059, System.Net.HttpStatusCode.BadRequest, "Order Not Found!!!");
            // }
            // var update = Builders<OrderCollection>.Update
            //       .Set(p => p.OrderStatus, orderStatus);
            // await collection.UpdateOneAsync(f => f.OrderId == orderId && f.TenantId == tenantId, update, new UpdateOptions { IsUpsert = false });
            // //==================writeDb
            // order.OrderStatus = orderStatus;
            // _orders.Update(order);
            // await _uow.SaveChangesAsync();
            // return orderRead;
            var collection = _orderingReadDbContext.OrderCollection;
            var order = await _orders.FirstOrDefaultAsync(f => f.Id == orderId && f.TenantId == tenantId);
            if (order != null)
            {
                order.OrderStatus = orderStatus;
                _orders.Update(order);
                try
                {
                    await _uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync();
                    }
                    await UpdateOneStatusAsync(tenantId, orderId, orderStatus);
                }
                UpdateResult updateResult;
                do
                {
                    var loaded = await collection.Find(w => w.OrderId == orderId && w.TenantId == tenantId).FirstOrDefaultAsync();
                    var version = loaded.Version;
                    loaded.OrderStatusLogs.Add(orderStatus);
                    loaded.Version++;
                    var update = Builders<OrderCollection>.Update
                        .Set(p => p.OrderStatus, orderStatus)
                        .Set(p => p.OrderStatusName, orderStatus.ToString())
                        .Set(p => p.OrderStatusLogs, loaded.OrderStatusLogs)
                        .Set(p => p.Version, loaded.Version);

                    updateResult = await collection.UpdateOneAsync(f => f.OrderId == orderId && f.TenantId == tenantId && f.Version == version, update, new UpdateOptions { IsUpsert = false });
                } while (updateResult.ModifiedCount != 1);
            }
            var orderRead = await collection.Find(f => f.OrderId == orderId && f.TenantId == tenantId).FirstOrDefaultAsync();
            return orderRead;
        }

        public async Task UpdateOrderStatus(List<long> orderIds, OrderStatus orderStatus)
        {
            var collection = _orderingReadDbContext.OrderCollection;
            var update = Builders<OrderCollection>.Update
                  .Set(p => p.OrderStatus, orderStatus);
            await collection.UpdateManyAsync(f => orderIds.Contains(f.OrderId), update, new UpdateOptions { IsUpsert = false });
            //==================writeDb
            var orders = await _orders.Where(f => orderIds.Contains(f.Id)).ToListAsync();

            if (orders != null && orders.Count > 0)
            {
                foreach (var item in orders)
                {
                    item.OrderStatus = orderStatus;
                }
                _orders.UpdateRange(orders);
                await _uow.SaveChangesAsync();
            }
        }
        public async Task UpdateOneStatusAsync(string tenantId, long orderId, OrderStatus orderStatus)
        {
            var collection = _orderingReadDbContext.OrderCollection;
            var order = await _orders.FirstOrDefaultAsync(f => f.Id == orderId && f.TenantId == tenantId);
            if (order != null)
            {
                order.OrderStatus = orderStatus;
                _orders.Update(order);
                try
                {
                    await _uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync();
                    }
                    await UpdateOneStatusAsync(tenantId, orderId, orderStatus);
                }
                UpdateResult updateResult;
                do
                {
                    var loaded = await collection.Find(w => w.OrderId == orderId && w.TenantId == tenantId).FirstOrDefaultAsync();
                    var version = loaded.Version;
                    loaded.OrderStatusLogs.Add(orderStatus);
                    loaded.Version++;
                    var update = Builders<OrderCollection>.Update
                        .Set(p => p.OrderStatus, orderStatus)
                        .Set(p => p.OrderStatusName, orderStatus.ToString())
                        .Set(p => p.OrderStatusLogs, loaded.OrderStatusLogs)
                        .Set(p => p.Version, loaded.Version);

                    updateResult = await collection.UpdateOneAsync(f => f.OrderId == orderId && f.TenantId == tenantId && f.Version == version, update, new UpdateOptions { IsUpsert = false });
                } while (updateResult.ModifiedCount != 1);
            }
        }
        public async Task<OrderCollection> GetOneByCorrelationId(string correlationId)
        {
            var collection = _orderingReadDbContext.OrderCollection;
            var res = await collection.Find(f => f.CorrelationId == correlationId).FirstOrDefaultAsync();
            return res;
        }

        public async Task<bool> CheckExistingStatus(long orderId, List<OrderStatus> orderStatuses)
        {
            var collection = _orderingReadDbContext.OrderCollection.AsQueryable();
            var query = collection.Where(a => a.OrderId == orderId);
            foreach (var item in orderStatuses)
            {
                query = query.Where(w => w.OrderStatusLogs.Contains(item));
            }
            var res = await query.AnyAsync();
            return res;
        }
    }
}