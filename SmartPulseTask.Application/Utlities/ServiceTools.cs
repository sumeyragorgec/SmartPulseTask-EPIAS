using Microsoft.Extensions.DependencyInjection;
using System;

namespace SmartPulseTask.Application.Utlities;
using Microsoft.Extensions.DependencyInjection;
 
public static class ServiceTool
{
    private static IServiceProvider? _serviceProvider;

    public static void Create(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static T? GetService<T>() where T : class
    {
        try
        {
            return _serviceProvider.GetService<T>();
        }
        catch (InvalidOperationException)
        {
            using var scope = _serviceProvider.CreateScope();
            return scope.ServiceProvider.GetService<T>();
        }
    }

    public static T GetRequiredService<T>() where T : class
    {
        return GetService<T>() ?? throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
    }

    public static IServiceScope CreateScope()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("ServiceTool is not initialized.");

        return _serviceProvider.CreateScope();
    }

    public static T? GetScopedService<T>(IServiceProvider? serviceProvider = null) where T : class
    {
        var provider = serviceProvider ?? _serviceProvider;
        if (provider == null)
            return null;

        try
        {
            return provider.GetService<T>();
        }
        catch
        {
            return null;
        }
    }
}
