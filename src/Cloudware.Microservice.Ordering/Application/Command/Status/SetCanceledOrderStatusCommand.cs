namespace Cloudware.Microservice.Ordering.Application.Command.Status
{
    public record SetCanceledOrderStatusCommand(string TenantId,long OrderId);
 
}