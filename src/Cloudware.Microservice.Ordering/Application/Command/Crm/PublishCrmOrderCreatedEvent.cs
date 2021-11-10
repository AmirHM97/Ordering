namespace Cloudware.Microservice.Ordering.Application.Command.Crm
{
    public record PublishCrmOrderCreatedEvent(string TenantId,long OrderId);
    
}