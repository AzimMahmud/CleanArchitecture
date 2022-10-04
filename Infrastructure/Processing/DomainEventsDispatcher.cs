using Application.Configuration.DomainEvents;
using Autofac;
using Autofac.Core;
using Core.Common;
using Infrastructure.Database;
using Infrastructure.Processing.Outbox;
using MediatR;
using Newtonsoft.Json;

namespace Infrastructure.Processing;

public class DomainEventsDispatcher : IDomainEventsDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILifetimeScope _scope;
    private readonly AppDbContext _dbContext;

    public DomainEventsDispatcher(IMediator mediator, ILifetimeScope scope, AppDbContext dbContext)
    {
        _mediator = mediator;
        _scope = scope;
        _dbContext = dbContext;
    }

    public async Task DispatchEventsAsync()
    {
        var domainEntities = _dbContext.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();


        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();


        var domainEventNotifications = new List<IDomainEventNotification<IDomainEvent>>();

        foreach (var domainEvent in domainEvents)
        {
            Type domainEventNotificationType = typeof(IDomainEventNotification<>);

            var domainNotificationWithGenericType = domainEventNotificationType.MakeGenericType(domainEvent.GetType());
            var domainNotification = _scope.ResolveOptional(domainNotificationWithGenericType, new List<Parameter>
            {
                new NamedParameter("domainEvent", domainEvent)
            });

            if (domainNotification is not null)
            {
                domainEventNotifications.Add(domainNotification as IDomainEventNotification<IDomainEvent>);
            }
        }


        domainEntities
            .ForEach(entity => entity.Entity.ClearDomainEvents());
        
        var tasks = domainEvents
            .Select(async domainEvent => await _mediator.Publish(domainEvent));

        await Task.WhenAll(tasks);

        foreach (var domainEventNotification in domainEventNotifications)
        {
            var type = domainEventNotification.GetType().FullName!;
            var data = JsonConvert.SerializeObject(domainEventNotification);

            OutboxMessage outboxMessage = new OutboxMessage(
                domainEventNotification.DomainEvent.OccurredOn,
                type!,
                data);

            _dbContext.OutboxMessages.Add(outboxMessage);
        }
    }
}