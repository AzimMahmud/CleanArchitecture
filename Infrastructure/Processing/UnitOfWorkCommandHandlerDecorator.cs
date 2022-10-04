using Application.Configuration.Commands;
using Core.Common;
using Infrastructure.Database;
using Infrastructure.Processing.InternalCommands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Processing;

public class UnitOfWorkCommandHandlerDecorator<T> : ICommandHandler<T> where T : ICommand
{
    private readonly ICommandHandler<T> _decorated;

    private readonly IUnitOfWork _unitOfWork;

    private readonly AppDbContext _dbContext;

    public UnitOfWorkCommandHandlerDecorator(ICommandHandler<T> decorated, IUnitOfWork unitOfWork,
        AppDbContext dbContext)
    {
        _decorated = decorated;
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(T command, CancellationToken cancellationToken)
    {
        await _decorated.Handle(command, cancellationToken);

        if (command is InternalCommandBase)
        {
            var internalCommand =
                await _dbContext.InternalCommands
                    .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (internalCommand is not null)
            {
                internalCommand.ProcessedDate = DateTime.UtcNow;
            }

            await _unitOfWork.CommitAsync(cancellationToken);
        }

        return Unit.Value;
    }
}