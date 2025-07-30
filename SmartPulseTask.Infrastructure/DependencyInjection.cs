using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartPulseTask.Application.Interfaces;
using SmartPulseTask.Infrastructure.Configuration;
using SmartPulseTask.Infrastructure.Services;

namespace SmartPulseTask.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EpiasApiSettings>(
            configuration.GetSection(EpiasApiSettings.SectionName));
        services.Configure<CacheSettings>(
            configuration.GetSection(CacheSettings.SectionName));

        services.AddMemoryCache();

        services.AddHttpClient<EpiasAuthenticationService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<EpiasDataService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddScoped<IEpiasAuthenticationService, EpiasAuthenticationService>();
        services.AddScoped<IEpiasDataService, EpiasDataService>();
        services.AddSingleton<ITgtTokenCache, MemoryTgtTokenCache>();

        return services;
    }
}