using Cloudware.Microservice.Ordering.Model;

namespace Cloudware.Microservice.Ordering.Application.Command.Status
{
    public record ChangeOrderStatusCommand(string TenantId,long OrderId,OrderStatus OrderStatus);
    
}