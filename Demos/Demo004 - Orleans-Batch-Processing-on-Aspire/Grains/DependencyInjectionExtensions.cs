using BatchProcessing.Abstractions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BatchProcessing.Grains;

public static class DependencyInjectionExtensions
{
    public static void AddBatchProcessingEngine(this IHostApplicationBuilder builder) => builder.Services.AddBatchProcessingEngine(builder.Configuration);

    public static IServiceCollection AddBatchProcessingEngine(this IServiceCollection services, IConfigurationManager builderConfiguration)
    {
        services.Configure<EngineConfig>(builderConfiguration.GetSection("Engine"));
        return services;
    }
}