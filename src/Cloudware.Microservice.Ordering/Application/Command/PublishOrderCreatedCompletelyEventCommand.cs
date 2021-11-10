namespace Cloudware.Microservice.Ordering.Application.Command
{
    public record PublishOrderCreatedCompletelyEventCommand(long OrderId,string TenantId);
 
}