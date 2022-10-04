namespace Infrastructure.Processing.Outbox;

public record OutboxMessageDto(Guid Id, string Type, string Date);
