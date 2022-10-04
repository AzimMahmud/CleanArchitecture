using Application.Configuration.Commands;
using Core.Common;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Processing;

public class UnitOfWorkCommandHandlerWithResultDecorator<T, TResult> : ICommandHandler<T, TResult> where T : ICommand<TResult>
{
    private readonly ICommandHandler<T, TResult> _decorated;

    private readonly IUnitOfWork _unitOfWork;

    private readonly AppDbContext _dbContext;

    public UnitOfWorkCommandHandlerWithResultDecorator(
        ICommandHandler<T, TResult> decorated, 
        IUnitOfWork unitOfWork, 
        AppDbContext dbContext)
    {
        _decorated = decorated;
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
    }
    
    public async Task<TResult> Handle(T command, CancellationToken cancellationToken)
    {
        var result = await _decorated.Handle(command, cancellationToken);

        if (command is InternalCommandBase<TResult>)
        {
            var internalCommand = await _dbContext.InternalCommands
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken: cancellationToken);

            if (internalCommand != null)
            {
                internalCommand.ProcessedDate = DateTime.UtcNow;
            }
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return result;
    }
}