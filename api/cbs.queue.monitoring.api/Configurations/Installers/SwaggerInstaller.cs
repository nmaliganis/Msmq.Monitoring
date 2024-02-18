using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace cbs.queue.monitoring.api.Configurations.Installers;

internal static class SwaggerInstaller
{
    public static IServiceCollection AddSwaggerInstaller(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "cbs.monitoring.api - HTTP API",
                Version = "v1",
                Description = "The Catalog Microservice HTTP API for cbs.monitoring.api service",
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerInstaller(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "cbs.monitoring.api - HTTP API");
        });

        return app;
    }
}