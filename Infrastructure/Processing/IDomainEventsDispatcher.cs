namespace Infrastructure.Processing;

public interface IDomainEventsDispatcher
{
    Task DispatchEventsAsync();
}