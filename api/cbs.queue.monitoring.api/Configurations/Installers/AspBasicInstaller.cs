using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace cbs.queue.monitoring.api.Configurations.Installers;

internal static class AspBasicInstaller
{
    private const string CorsPolicyName = "AllowSpecificOrigins";

    public static IServiceCollection AddAspBasicInstaller(this IServiceCollection services)
    {
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddScoped<IUrlHelper>(implementationFactory =>
        {
            var actionContext = implementationFactory.GetService<IActionContextAccessor>()
          .ActionContext;
            return new UrlHelper(actionContext);
        });

        services.AddScoped<IUrlHelper>(x =>
        {
            var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
            var factory = x.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(actionContext);
        });

        services.AddMemoryCache();

        services.AddResponseCaching();

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName,
          builderCors =>
          {
              builderCors
              .SetIsOriginAllowed((host) => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowAnyOrigin();
          });
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.Configure<IpRateLimitOptions>((options) =>
        {
            options.GeneralRules = new System.Collections.Generic.List<RateLimitRule>()
          {
        new RateLimitRule()
        {
          Endpoint = "*",
          Limit = 1000,
          Period = "5m"
        },
        new RateLimitRule()
        {
          Endpoint = "*",
          Limit = 200,
          Period = "10s"
        }
          };
        });

        services.AddHttpCacheHeaders(
          (expirationModelOptions)
            =>
          {
              expirationModelOptions.MaxAge = 60;
              expirationModelOptions.SharedMaxAge = 30;
          },
          (validationModelOptions)
            =>
          {
              validationModelOptions.MustRevalidate = true;
              validationModelOptions.ProxyRevalidate = true;
          });

        return services;
    }

    public static IApplicationBuilder UseAspBasicInstaller(this IApplicationBuilder app)
    {
        app.UseCors(CorsPolicyName);
        app.UseResponseCaching();
        app.UseHttpCacheHeaders();
        app.UseCookiePolicy();
        app.UseHttpsRedirection();

        app.Use(async (context, next) =>
        {
            if (context.Request.Method == HttpMethods.Options)
            {
                context.Response.StatusCode = 200;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:80");
                context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:8080");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET,HEAD,OPTIONS,POST,PUT,DELETE");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");
                context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                await context.Response.WriteAsync("OK");
            }
            else
            {
                await next();
            }
        });

        return app;
    }
}//Class : AspBasicInstaller