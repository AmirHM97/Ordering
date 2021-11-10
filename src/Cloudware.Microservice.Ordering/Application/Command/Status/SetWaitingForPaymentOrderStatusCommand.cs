namespace Cloudware.Microservice.Ordering.Application.Command.Status
{
    public record SetWaitingForPaymentOrderStatusCommand(long OrderId,string TenantId);
   
}