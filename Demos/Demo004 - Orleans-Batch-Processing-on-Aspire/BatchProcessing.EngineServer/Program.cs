using BatchProcessing.Domain;
using BatchProcessing.Grains;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var redisConfigurationOptions = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("redis")!);

builder.AddServiceDefaults();

builder.AddDomainInfrastructure();
builder.AddBatchProcessingEngine();

builder.AddKeyedRedisClient("redis");

builder.UseOrleans(siloBuilder =>
{
    siloBuilder.AddDistributedGrainDirectory();

    siloBuilder.UseRedisClustering(options => options.ConfigurationOptions = redisConfigurationOptions);
    siloBuilder.UseRedisGrainDirectoryAsDefault(options => options.ConfigurationOptions = redisConfigurationOptions);
    siloBuilder.UseSiloMetadataWithRegion(Environment.GetEnvironmentVariable("REGION_NAME")!);

    if (builder.Environment.IsDevelopment())
    {
        siloBuilder.UseDashboard(options => options.HostSelf = false);
    }
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();
