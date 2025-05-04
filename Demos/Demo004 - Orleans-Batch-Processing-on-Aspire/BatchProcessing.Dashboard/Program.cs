using Orleans.Runtime.MembershipService.SiloMetadata;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var redisConfigurationOptions = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("redis")!);

builder.AddServiceDefaults();

builder.AddKeyedRedisClient("redis");

builder.UseOrleans(siloBuilder =>
{
    siloBuilder.AddDistributedGrainDirectory();

    siloBuilder.UseRedisClustering(options => options.ConfigurationOptions = redisConfigurationOptions);
    siloBuilder.UseRedisGrainDirectoryAsDefault(options => options.ConfigurationOptions = redisConfigurationOptions);
    siloBuilder.UseSiloMetadata();

    if (builder.Environment.IsDevelopment())
    {
        siloBuilder.UseDashboard(options => options.HostSelf = true);
    }
});

var app = builder.Build();

app.MapDefaultEndpoints();
app.Map("/dashboard", x => x.UseOrleansDashboard());

app.Run();