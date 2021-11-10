using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using Cloudware.Utilities.Contract.PaymentGatway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Command
{
    public class CreatePrepurchaseInformationCommand : IRequestType
    {
        public long Id { get; set; }
        public string TenantId { get; set; }
        public string UserId { get; set; }
        public Device Device { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public CreatePrepurchaseInformationCommand(long id, string tenantId, string userId, Device device, string firstName, string lastName)
        {
            Id = id;
            TenantId = tenantId;
            UserId = userId;
            Device = device;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
