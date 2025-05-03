using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");
var mongoDb = builder.AddMongoDB("mongoDb");

if (builder.Environment.IsDevelopment())
{
    // No need to persist right now
    //mongoDb.WithDataVolume("prototyping-mongo");
}

var orleans = builder.AddOrleans("orleans-engine")
    .WithClusterId("orleans-cluster")
    .WithServiceId("BatchProcessing")
    .WithClustering(redis);

builder.AddProject<Projects.BatchProcessing_Dashboard>("dashboard")
    .WithReference(redis)
    .WithReference(orleans)
    .WaitFor(redis);

var engine = builder.AddProject<Projects.BatchProcessing_EngineServer>("engine")
    .WithReference(redis)
    .WithReference(orleans)
    .WithReference(mongoDb)
    //.WithReplicas(2);
    .WaitFor(redis)
    .WaitFor(mongoDb);

builder.AddProject<Projects.BatchProcessing_EngineServer>("engine-na", "na-https")
    .WithReference(redis)
    .WithReference(orleans)
    .WithReference(mongoDb)
    .WithReplicas(2)
    .WaitFor(redis)
    .WaitFor(mongoDb);

builder.AddProject<Projects.BatchProcessing_EngineServer>("engine-we", "we-https")
    .WithReference(redis)
    .WithReference(orleans)
    .WithReference(mongoDb)
    .WithReplicas(2)
    .WaitFor(redis)
    .WaitFor(mongoDb);


builder.AddProject<Projects.BatchProcessing_EngineServer>("engine-au", "au-https")
    .WithReference(redis)
    .WithReference(orleans)
    .WithReference(mongoDb)
    .WithReplicas(2)
    .WaitFor(redis)
    .WaitFor(mongoDb);

builder.AddProject<Projects.BatchProcessing_WebApp>("web-frontend")
    .WithExternalHttpEndpoints()
    .WithReference(redis)
    .WithReference(orleans.AsClient())
    .WaitFor(engine);

builder.Build().Run();