using Application.Configuration.Commands;
using Application.Configuration.Processing;
using Infrastructure.Database;
using Infrastructure.Processing.InternalCommands;
using Newtonsoft.Json;


namespace Infrastructure.Processing;

public class CommandsScheduler : ICommandsScheduler
{
    private readonly AppDbContext _dbContext;

    public CommandsScheduler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnqueueAsync<T>(ICommand<T> command)
    {
        await _dbContext.Set<InternalCommand>()
            .AddAsync(new InternalCommand
            {
                Id = command.Id,
                EnqueueDate = DateTime.UtcNow,
                Type = command.GetType().FullName!,
                Data = JsonConvert.SerializeObject(command)
            });

        await _dbContext.SaveChangesAsync();
    }
}