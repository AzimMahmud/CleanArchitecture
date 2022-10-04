using System.Text.Json.Serialization;
using Application.Configuration.Commands;
using Application.Configuration.DomainEvents;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Processing.Outbox;

public class ProcessOutboxCommandHandler : ICommandHandler<ProcessOutboxCommand, Unit>
{
    private readonly AppDbContext _dbContext;
    private readonly IMediator _mediator;

    public ProcessOutboxCommandHandler(AppDbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(ProcessOutboxCommand command, CancellationToken cancellationToken)
    {
        var messageList = await _dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedDate == null)
            .ToListAsync(cancellationToken);


        if (messageList.Any())
        {
            foreach (var message in messageList)
            {
                Type? type = Assemblies.Application.GetType(message.Type);

                var request = JsonConvert.DeserializeObject(message.Data, type!) as IDomainEventNotification;

                using (LogContext.Push(new OutboxMessageContextEnricher(request!)))
                {
                    await _mediator.Publish(request!, cancellationToken);

                    message.ProcessedDate = DateTime.UtcNow;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
    
    
    private class OutboxMessageContextEnricher : ILogEventEnricher
    {
        private readonly IDomainEventNotification _notification;

        public OutboxMessageContextEnricher(IDomainEventNotification notification)
        {
            _notification = notification;
        }
    
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddOrUpdateProperty(new LogEventProperty("Context", new ScalarValue($"OutboxMessage:{_notification.Id.ToString()}")));
        }
    }
}