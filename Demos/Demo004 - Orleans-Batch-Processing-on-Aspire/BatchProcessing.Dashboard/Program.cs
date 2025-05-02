using Orleans.Runtime.MembershipService.SiloMetadata;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddKeyedRedisClient("redis");

builder.UseOrleans(siloBuilder =>
{
    siloBuilder.UseSiloMetadata();
    siloBuilder.UseRedisClustering("redis");

    if (builder.Environment.IsDevelopment())
    {
        siloBuilder.UseDashboard(options => options.HostSelf = true);
    }
});

var app = builder.Build();

app.MapDefaultEndpoints();
app.Map("/dashboard", x => x.UseOrleansDashboard());

app.Run();