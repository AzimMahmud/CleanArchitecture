using MediatR;

namespace Core.SeedWork;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}