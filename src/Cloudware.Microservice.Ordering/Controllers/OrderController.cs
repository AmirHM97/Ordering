using Cloudware.Microservice.Ordering.Application.Command;
using Cloudware.Microservice.Ordering.Application.Query;
using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Microservice.Ordering.Infrastructure.Extension;
using Cloudware.Microservice.Ordering.Infrastructure.Services;
using Cloudware.Microservice.Ordering.Model;
using Cloudware.Microservice.Ordering.Model.ReadDb;
using Cloudware.Utilities.Common.Filter;
using Cloudware.Utilities.Configure.Microservice;
using Cloudware.Utilities.Configure.Microservice.Services;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.Ordering;
using Cloudware.Utilities.Contract.PaymentGatway;
using Cloudware.Utilities.Contract.Shop.Notification;
using Cloudware.Utilities.Contract.Wallet;
using DocumentFormat.OpenXml.InkML;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test;
using static Cloudware.Microservice.Ordering.Application.Command.CreatePrepurchaseInformationCommandConsumer;
using static Cloudware.Microservice.Ordering.Application.Command.ExportOrdersToExcelCommandConsumer;
using static Cloudware.Microservice.Ordering.Application.Query.GetOrderByIdQueryConsumer;
using static Cloudware.Microservice.Ordering.Application.Query.GetOrderDetailStatusQueryConsumer;
using static Cloudware.Microservice.Ordering.Application.Query.GetOrderListQueryConsumer;
using static Cloudware.Microservice.Ordering.Application.Query.GetOrderStatusFiltersQueryConsumer;

namespace Cloudware.Microservice.Ordering.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ClwControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<GetOrderStatusFiltersQuery> _getOrderStatusFiltersQuery;
        private readonly IRequestClient<GetFilteredOrdersListOfUserByOrderStatusQuery> _getFilteredOrdersListOfUserByOrderStatusQuery;
        private readonly IRequestClient<GetOrderByIdQuery> _getOrderByIdQuery;
        private readonly IRequestClient<GetOrderDetailStatusQuery> _getOrderDetailStatusQuery;
        private readonly IMediator _mediator;
        private readonly IUserInformationService _userInformationService;
        private readonly IOrderService _orderService;
        private readonly IOrderingReadDbContext _orderingReadDbContext;


        public OrderController(IPublishEndpoint publishEndpoint, IRequestClient<GetOrderStatusFiltersQuery> getOrderStatusFiltersQuery, IRequestClient<GetOrderByIdQuery> getOrderByIdQuery, IRequestClient<GetOrderDetailStatusQuery> getOrderDetailStatusQuery, IMediator mediator,
            IUserInformationService userInformationService, IOrderService orderService, IOrderingReadDbContext orderingReadDbContext, IRequestClient<GetFilteredOrdersListOfUserByOrderStatusQuery> getFilteredOrdersListOfUserByOrderStatusQuery)
        {
            _publishEndpoint = publishEndpoint;
            _getOrderStatusFiltersQuery = getOrderStatusFiltersQuery;
            _getOrderByIdQuery = getOrderByIdQuery;
            _getOrderDetailStatusQuery = getOrderDetailStatusQuery;
            _mediator = mediator;
            _userInformationService = userInformationService;
            _orderService = orderService;
            _orderingReadDbContext = orderingReadDbContext;
            _getFilteredOrdersListOfUserByOrderStatusQuery = getFilteredOrdersListOfUserByOrderStatusQuery;
        }
        [HttpGet]
        public async Task<ActionResult> GetOrderStatusFilters()
        {
            var result = await _getOrderStatusFiltersQuery.GetResponse<GetOrderStatusFiltersQueryResponse>(new GetOrderStatusFiltersQuery(UserId));
            return Ok(result.Message.OrderStatusItems);
        }
        [HttpGet]
        public async Task<ActionResult<GetOrderDetailStatusQueryResponse>> GetOrderDetailsStatus(long orderId)
        {
            var result = await _getOrderDetailStatusQuery.GetResponse<GetOrderDetailStatusQueryResponse>(new GetOrderDetailStatusQuery(orderId));
            return Ok(result.Message);
        }
        [HttpGet]
        public async Task<ActionResult<GetFilteredOrdersListOfUserByOrderStatusQueryResponse>> FilterOrdersByStatus(OrderStatus statusId, int pageSize = 5, int pageNumber = 1)
        {

            var result = await _getFilteredOrdersListOfUserByOrderStatusQuery
                .GetResponse<GetFilteredOrdersListOfUserByOrderStatusQueryResponse>(new GetFilteredOrdersListOfUserByOrderStatusQuery(UserId, statusId, pageSize, pageNumber));
            return Ok(result.Message.Orders);
        }

        [HttpGet]
        public async Task<ActionResult> GetOrderbyId(long orderId)
        {
            var result = await _getOrderByIdQuery.GetResponse<GetOrderByIdQueryResponse>(new GetOrderByIdQuery(orderId, true));

            return Ok(result.Message);
        }

        [HttpGet]
        public async Task<ActionResult> PayOrder(long orderId)
        {
            Device device = Device.pwa;
            if (Request.Headers.TryGetValue("x-device", out var deviceValue))
            {
                device = OrderingExtensions.GetDevice(deviceValue);
            }
            var getUserInfo = await _userInformationService.GetUserInfo();
            var firstName = getUserInfo.Firstname;
            var lastName = getUserInfo.Lastname;
            var _createOrderCommand = _mediator.CreateRequestClient<CreatePrepurchaseInformationCommand>();
            var orderInfo = await _createOrderCommand.GetResponse<CreatePrepurchaseInformationCommandResponse>(new CreatePrepurchaseInformationCommand(orderId, TenantId, UserId, device, firstName, lastName));
            return Ok(orderInfo.Message);
        }

        [HttpGet]
        public async Task<IActionResult> ExportToXlsxOrders()
        {
            var getOrderListQueryReq = _mediator.CreateRequestClient<GetOrderListQuery>();
            var getOrderListQueryRes = await getOrderListQueryReq.GetResponse<GetOrderListQueryResponse>(new GetOrderListQuery());

            var exportOrdersToExcelCommandReq = _mediator.CreateRequestClient<ExportOrdersToExcelCommand>();
            var exportOrdersToExcelCommandResponse = await exportOrdersToExcelCommandReq.GetResponse<ExportOrdersToExcelCommandResponse>(new ExportOrdersToExcelCommand { Orders = getOrderListQueryRes.Message.Orders });

            return File(exportOrdersToExcelCommandResponse.Message.Content, exportOrdersToExcelCommandResponse.Message.ContentType, "لیست سفارشات Vip.xlsx");
        }

      
    }
}
