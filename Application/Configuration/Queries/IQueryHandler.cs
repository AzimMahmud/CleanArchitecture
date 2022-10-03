using MediatR;

namespace Application.Configuration.Queries;

public interface IQueryHandler<in TQuery, TResult> :
    IRequestHandler<TQuery, TResult> where TQuery : IRequest<TResult>
{
}