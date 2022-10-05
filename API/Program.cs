
using API.Common;
using API.Configuration;
using Application.Configuration.Behavior.Validation;
using Application.Configuration.Emails;
using Core.Common;
using Hellang.Middleware.ProblemDetails;
using Infrastructure;
using Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Serilog.Formatting.Compact;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

string AppConnectionString = builder.Configuration["AppConnectionString"];

var logger = ConfigureLogger();
logger.Information("Logger configured");

builder.Services.AddControllers();
            
builder.Services.AddMemoryCache();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddProblemDetails(x =>
{
    x.Map<InvalidCommandException>(ex => new InvalidCommandProblemDetails(ex));
    x.Map<BusinessRuleValidationException>(ex => new BusinessRuleValidationExceptionProblemDetails(ex));
});
            

builder.Services.AddHttpContextAccessor();
var serviceProvider = builder.Services.BuildServiceProvider();

var executionContextAccessor = new ExecutionContextAccessor(serviceProvider.GetService<IHttpContextAccessor>());

var children = builder.Configuration.GetSection("Caching").GetChildren();
var cachingConfiguration = children.ToDictionary(child => child.Key, child => TimeSpan.Parse(child.Value));
var emailsSettings = builder.Configuration.GetSection("EmailsSettings").Get<EmailsSettings>();
var memoryCache = serviceProvider.GetService<IMemoryCache>();
ApplicationStartup.Initialize(
    builder.Services, 
    AppConnectionString,
    new MemoryCacheStore(memoryCache, cachingConfiguration),
    null,
    emailsSettings,
    logger,
    executionContextAccessor);



var app = builder.Build();



app.UseMiddleware<CorrelationMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseProblemDetails();
}

app.UseRouting();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.UseSwaggerDocumentation();

app.Run();


static ILogger ConfigureLogger()
{
    return new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Context}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.RollingFile(new CompactJsonFormatter(), "logs/logs")
        .CreateLogger();
}
