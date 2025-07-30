using Microsoft.Extensions.DependencyInjection;
using SmartPulseTask.Application.Interfaces;
using SmartPulseTask.Application.Services;
using SmartPulseTask.Application.UseCases;
using SmartPulseTask.Infrastructure.Services;

namespace SmartPulseTask.Application;


public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITransactionAnalysisService, TransactionAnalysisService>();
        services.AddScoped<IContractDateParser, ContractDateParser>();
        services.AddScoped<IAnalyzeTransactionsUseCase, AnalyzeTransactionsUseCase>();
        services.AddScoped<IAnalyzeTransactionsWithTokenUseCase, AnalyzeTransactionsWithTokenUseCase>();
        services.AddScoped<IGetTgtTokenUseCase, GetTgtTokenUseCase>();
        services.AddScoped<IGetTgtTokenUseCase, GetTgtTokenUseCase>();

        return services;
    }
}
