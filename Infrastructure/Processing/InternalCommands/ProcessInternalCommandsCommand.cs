using Application.Configuration.Commands;
using Infrastructure.Processing.Outbox;
using MediatR;

namespace Infrastructure.Processing.InternalCommands;

internal class ProcessInternalCommandsCommand : CommandBase<Unit> , IRecurringCommand
{
}