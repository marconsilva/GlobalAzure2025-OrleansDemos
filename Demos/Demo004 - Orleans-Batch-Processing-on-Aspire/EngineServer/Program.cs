using BatchProcessing.Domain;
using BatchProcessing.Grains;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var redisConfigurationOptions = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("redis")!);
var dashboard = Environment.GetEnvironmentVariable("ORLEANS_DASHBOARD") == "HostSelf";

builder.AddServiceDefaults();

builder.AddDomainInfrastructure();
builder.AddBatchProcessingEngine();

builder.AddKeyedRedisClient("redis");

builder.UseOrleans(siloBuilder =>
{
    siloBuilder.AddActivityPropagation();
    siloBuilder.AddDistributedGrainDirectory();

    siloBuilder.UseRedisClustering(options => options.ConfigurationOptions = redisConfigurationOptions);
    siloBuilder.UseRedisGrainDirectoryAsDefault(options => options.ConfigurationOptions = redisConfigurationOptions);
    siloBuilder.UseSiloMetadataWithRegion(Environment.GetEnvironmentVariable("REGION_NAME")!);

    if (builder.Environment.IsDevelopment())
    {
        siloBuilder.UseDashboard(options => options.HostSelf = dashboard);
    }
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (builder.Environment.IsDevelopment() && dashboard)
{
    app.Map("", x => x.UseOrleansDashboard());
}

app.Run();
