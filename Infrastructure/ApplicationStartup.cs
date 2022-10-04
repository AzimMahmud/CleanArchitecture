using Application.Configuration;
using Application.Configuration.Emails;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Infrastructure.Caching;
using Infrastructure.Database;
using Infrastructure.Domain;
using Infrastructure.Emails;
using Infrastructure.Logging;
using Infrastructure.Processing;
using Infrastructure.Quartz;
using Infrastructure.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Impl;
using Serilog;

namespace Infrastructure;

public class ApplicationStartup
{
    public static IServiceProvider Initialize(
        IServiceCollection services,
        string connectionString,
        ICacheStore cacheStore,
        IEmailSender? emailSender,
        EmailsSettings emailsSettings,
        ILogger logger,
        IExecutionContextAccessor executionContextAccessor,
        bool runQuartz = true
    )
    {
        if (runQuartz)
        {
            StartQuartz(connectionString, emailsSettings, logger, executionContextAccessor);
        }


        services.AddSingleton(cacheStore);

        var serviceProvider = CreateAutofacServiceProvider(services, connectionString, emailSender, emailsSettings,
            logger,
            executionContextAccessor);

        return serviceProvider;
    }


    private static IServiceProvider CreateAutofacServiceProvider(IServiceCollection services, string connectionString,
        IEmailSender? emailSender, EmailsSettings emailsSettings, ILogger logger,
        IExecutionContextAccessor executionContextAccessor)
    {
        var container = new ContainerBuilder();

        container.Populate(services);

        container.RegisterModule(new LoggingModule(logger));
        container.RegisterModule(new MediatorModule());
        container.RegisterModule(new DomainModule());

        container.RegisterModule(emailSender is not null
            ? new EmailModule(emailSender, emailsSettings)
            : new EmailModule(emailsSettings));

        container.RegisterModule(new ProcessingModule());

        container.RegisterInstance(executionContextAccessor);

        var buildContainer = container.Build();

        ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(buildContainer));

        var serviceProvider = new AutofacServiceProvider(buildContainer);

        CompositionRoot.SetContainer(buildContainer);

        return serviceProvider;
    }


    private static void StartQuartz(string connectionString, EmailsSettings emailsSettings, ILogger logger,
        IExecutionContextAccessor executionContextAccessor)
    {
        var schedulerFactory = new StdSchedulerFactory();
        var scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();

        var container = new ContainerBuilder();

        container.RegisterModule(new LoggingModule(logger));
        container.RegisterModule(new QuartzModule());
        container.RegisterModule(new MediatorModule());
        container.RegisterModule(new EmailModule(emailsSettings));
        container.RegisterModule(new ProcessingModule());

        container.RegisterInstance(executionContextAccessor);

        container.Register(c =>
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            dbContextOptionsBuilder.UseSqlServer(connectionString);

            dbContextOptionsBuilder.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();

            return new AppDbContext(dbContextOptionsBuilder.Options);
        }).AsSelf().InstancePerLifetimeScope();
    }
}

