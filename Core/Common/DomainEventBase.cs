namespace Core.Common;

public class DomainEventBase : IDomainEvent
{
    public DateTime OccurredOn { get; }
    
    public DomainEventBase()
    {
        this.OccurredOn = DateTime.Now;
    }
}