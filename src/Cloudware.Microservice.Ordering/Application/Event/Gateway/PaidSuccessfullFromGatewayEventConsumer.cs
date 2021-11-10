using Cloudware.Microservice.Ordering.Application.Query;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Ordering;
using Cloudware.Utilities.Contract.PaymentGatway;
using Cloudware.Utilities.Contract.Product.Stock;
using Cloudware.Utilities.Contract.Wallet;
using DnsClient.Internal;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Cloudware.Microservice.Ordering.Application.Query.GetOrderStockItemsQueryConsumer;

namespace Cloudware.Microservice.Ordering.Application.Event.Gateway
{
    public class PaidSuccessfullFromGatewayEventConsumer : IConsumer<PaidSuccessfullFromGatewayEvent>, IBusConsumerType
    {
        private readonly IRequestClient<GetOrderStockItemsQuery> _getOrderStockItemsQuery;
        private readonly IRequestClient<CheckStockQuery> _checkStockQuery;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PaidSuccessfullFromGatewayEventConsumer> _logger;

        public PaidSuccessfullFromGatewayEventConsumer(IRequestClient<GetOrderStockItemsQuery> getOrderStockItemsQuery, IRequestClient<CheckStockQuery> checkStockQuery, IServiceScopeFactory serviceScopeFactory, ILogger<PaidSuccessfullFromGatewayEventConsumer> logger)
        {
            _getOrderStockItemsQuery = getOrderStockItemsQuery;
            _checkStockQuery = checkStockQuery;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaidSuccessfullFromGatewayEvent> context)
        {
            var stockItems = await _getOrderStockItemsQuery.GetResponse<GetOrderStockItemsQueryResponse>(new GetOrderStockItemsQuery(context.Message.OrderId));
            if (stockItems.Message.OrderId != 0)
            {

                var checkStocResponse = await _checkStockQuery.GetResponse<CheckStockQueryResponse>(new CheckStockQuery { OrderId = context.Message.OrderId, CheckStockItems = stockItems.Message.CheckStockItems });
                if (checkStocResponse.Message.StockAvailable)
                {
                    _logger.LogInformation($"Order {context.Message.OrderId}'s Stock confirmed!!!");
                    //stock OK
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var publisher = scope.ServiceProvider.GetService<IPublishEndpoint>();
                        await publisher.Publish(new OrderStockConfirmedEvent
                        {
                            OrderId = context.Message.OrderId,
                            TransactionNumber = context.Message.TransactionNumber,
                            UserId = stockItems.Message.UserId,
                            TotalPrice = stockItems.Message.TotalPrice
                        });
                    }

                }
                else
                {
                    _logger.LogDebug($"Order {context.Message.OrderId}'s Stock Not confirmed with data {stockItems.Message}");

                    //stock Not OK
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var publisher = scope.ServiceProvider.GetService<IPublishEndpoint>();
                        await publisher.Publish(new OrderStockNotConfirmedEvent
                        {
                            OrderId = context.Message.OrderId,
                            TransactionNumber = context.Message.TransactionNumber,
                            UserId = stockItems.Message.UserId
                        });
                    }
                }
            }
            else
            {
                _logger.LogDebug($"Order {context.Message.OrderId} Does Not Exist!!!");
                _logger.LogError($"Order {context.Message.OrderId} Does Not Exist!!!");
            }
        }

    }
}
