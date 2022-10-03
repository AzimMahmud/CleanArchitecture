using System.Net.Http.Json;
using System.Security.AccessControl;
using Application.Configuration.Commands;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infrastructure.Processing.InternalCommands;

internal class ProcessInternalCommandsCommandHandler : ICommandHandler<ProcessInternalCommandsCommand, Unit>
{
    private readonly AppDbContext _dbContext;

    public ProcessInternalCommandsCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Unit> Handle(ProcessInternalCommandsCommand request, CancellationToken cancellationToken)
    {
        var internalCommandsList = _dbContext.InternalCommands
            .Where(x => x.ProcessedDate == null)
            .AsNoTracking()
            .Select(x => new InternalCommandDto
            {
                Type = x.Type,
                Data = x.Data
            })
            .ToList();
        
        foreach (var internalCommand in internalCommandsList)
        {
            Type type = Assemblies.Application.GetType(internalCommand.Type);
            dynamic commandToProcess = JsonConvert.DeserializeObject(internalCommand.Data, type);

            // await CommandsExecutor.Execute(commandToProcess);
        }

        return Unit.Value;
    }
    
    private class InternalCommandDto
    {
        public string Type { get; set; }
        public string Data { get; set; }
    }
}