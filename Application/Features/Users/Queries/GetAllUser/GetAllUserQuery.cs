using Application.Configuration.Queries;

namespace Application.Features.Users.Queries.GetAllUser;

public record GetAllUserQuery : IQuery<string>;


internal class GetAllUserQueryHandler : IQueryHandler<GetAllUserQuery, string>
{
    public async Task<string> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
    {
        return "Hello world";
    }
}


    
