using ExternalScaler;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.UseOrleansClient(clientBuilder =>
{
    clientBuilder.UseRedisClustering(builder.Configuration.GetConnectionString("redis"));
});

var app = builder.Build();
app.MapGrpcService<ScalerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.Run();
