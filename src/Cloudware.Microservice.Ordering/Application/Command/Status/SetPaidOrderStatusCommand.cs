using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Command
{
    public record SetPaidOrderStatusCommand(string TenantId,long OrderId):IRequestType;
   
}
