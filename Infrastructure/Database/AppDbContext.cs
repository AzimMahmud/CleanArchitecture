using Infrastructure.Processing.InternalCommands;
using Infrastructure.Processing.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContext : DbContext
{
    public DbSet<InternalCommand> InternalCommands { get; set; }
    
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}