using Application.Configuration.Queries;
using Autofac;
using MediatR;

namespace Infrastructure.Processing;

public static class QueriesExecutor
{
    public static async Task<TResult> Execute<TResult>(IQuery<TResult> query)
    {
        var scope = CompositionRoot.BeginLifetimeScope();

        var mediator = scope.Resolve<IMediator>();

        return await mediator.Send(query);
    }

}