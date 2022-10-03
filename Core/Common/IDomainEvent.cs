using MediatR;

namespace Core.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}