using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace cbs.queue.monitoring.api.Configurations.Installers;

internal static class RoutingVersioningInstaller
{
  public static IServiceCollection AddRoutingVersioningInstaller(this IServiceCollection services)
  {
    services.AddMvc(options =>
      {
        options.EnableEndpointRouting = false;
        options.RespectBrowserAcceptHeader = true;
        options.ReturnHttpNotAcceptable = true;
      })
      .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
      .AddNewtonsoftJson(options =>
      {
        options.SerializerSettings.ContractResolver =
          new DefaultContractResolver();
      })
      .AddFluentValidation();

    services.AddApiVersioning(o =>
    {
      o.ReportApiVersions = true;
      o.AssumeDefaultVersionWhenUnspecified = true;
      o.DefaultApiVersion = new ApiVersion(1, 0);
    });

    return services;
  }

  public static IApplicationBuilder UseRoutingVersioningInstaller(this IApplicationBuilder app)
  {
    app.UseRouting();
    app.UseApiVersioning();

    return app;
  }
}