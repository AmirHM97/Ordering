using Cloudware.Microservice.Ordering.Model.ReadDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Cloudware.Microservice.Ordering.Application.Query.GetOrderListQueryConsumer;

namespace Cloudware.Microservice.Ordering.Application.Command
{
    public class ExportOrdersToExcelCommand
    {
        public List<OrderCollection> Orders{ get; set; }
    }
}
