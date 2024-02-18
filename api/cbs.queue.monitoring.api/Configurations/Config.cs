using cbs.common.infrastructure.PropertyMappings;
using cbs.common.infrastructure.TypeHelpers;
using cbs.queue.monitoring.api.Helpers;
using cbs.queue.monitoring.contracts.ContractRepositories;
using cbs.queue.monitoring.contracts.V1.Messages;
using cbs.queue.monitoring.repository.Repositories.Messages;
using cbs.queue.monitoring.services.V1.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace cbs.queue.monitoring.api.Configurations;

internal static class Config
{
    internal static IServiceCollection ConfigureInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPropertyMappingService, PropertyMappingService>();
        services.AddSingleton<ITypeHelperService, TypeHelperService>();

        return services;
    }

    internal static IServiceCollection ConfigureContracts(this IServiceCollection services)
    {
        services.ConfigureMessageContracts();

        return services;
    }

    private static IServiceCollection ConfigureMessageContracts(this IServiceCollection services)
    {
        services.AddScoped<ICreateMessageProcessor, CreateMessageProcessor>();
        services.AddScoped<IUpdateMessageProcessor, UpdateMessageProcessor>();
        services.AddScoped<IDeleteMessageProcessor, DeleteMessageProcessor>();
        services.AddScoped<IGetMessageByIdProcessor, GetMessageByIdProcessor>();
        services.AddScoped<IGetMessagesProcessor, GetMessagesProcessor>();

        services.AddScoped<IMessageRepository, MessageRepository>();
        return services;
    }

    public static IServiceCollection AddCustomMvc(this IServiceCollection services)
    {
        // Add framework services.
        services.AddControllers(options =>
          {
              options.Filters.Add(typeof(HttpGlobalExceptionFilter));
          })
          // Added for functional tests
          .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
          builder => builder
            .SetIsOriginAllowed((host) => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
        });

        return services;
    }
}//Class : Config