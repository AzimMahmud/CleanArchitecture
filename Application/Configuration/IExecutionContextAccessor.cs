namespace Application.Configuration;

public interface IExecutionContextAccessor
{
    Guid CorrelationId { get; }

    bool IsAvailable { get; }
}