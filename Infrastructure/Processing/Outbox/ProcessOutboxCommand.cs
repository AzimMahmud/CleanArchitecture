using Application.Configuration.Commands;
using MediatR;

namespace Infrastructure.Processing.Outbox;

public class ProcessOutboxCommand : CommandBase<Unit>, IRecurringCommand
{
}