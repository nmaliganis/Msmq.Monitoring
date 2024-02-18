using AutoMapper;
using cbs.common.infrastructure.Types;
using cbs.queue.monitoring.api.Configurations.AutoMappingProfiles.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace cbs.queue.monitoring.api.Configurations.Installers;

internal static class AutoMapperInstaller
{
    public static IServiceCollection AddAutoMapperInstaller(this IServiceCollection services)
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            // Message
            cfg.AddProfile<MessageToMessageUiAutoMapperProfile>();
        });

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

        services.AddSingleton<IAutoMapper, AutoMapperAdapter>();

        return services;
    }
}//Class : AutoMapperInstaller