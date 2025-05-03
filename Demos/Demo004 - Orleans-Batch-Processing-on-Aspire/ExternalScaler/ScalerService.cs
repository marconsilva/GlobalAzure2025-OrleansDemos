using Externalscaler;
using Grpc.Core;

namespace ExternalScaler;

public class ScalerService(IClusterClient clusterClient, ILogger<ScalerService> logger) : Externalscaler.ExternalScaler.ExternalScalerBase
{
    private const string MetricName = "grainThreshold";
    private const string GrainTypeMetadata = "graintype";
    private const string SiloNameMetadata = "siloNameFilter";
    private const string UpperBoundMetadata = "upperbound";

    private IManagementGrain _managementGrain = clusterClient.GetGrain<IManagementGrain>(0);

    public override async Task<GetMetricsResponse> GetMetrics(GetMetricsRequest request, ServerCallContext context)
    {
        CheckRequestMetadata(request.ScaledObjectRef);

        var grainType = request.ScaledObjectRef.ScalerMetadata[GrainTypeMetadata];
        var siloNameFilter = request.ScaledObjectRef.ScalerMetadata[SiloNameMetadata];
        var upperbound = Convert.ToInt32(request.ScaledObjectRef.ScalerMetadata[UpperBoundMetadata]);

        var fnd = await GetGrainCountInCluster(grainType, siloNameFilter);

        long grainsPerSilo = (fnd.GrainCount > 0 && fnd.SiloCount > 0) ? (fnd.GrainCount / fnd.SiloCount) : 0;
        long metricValue = fnd.SiloCount;

        // scale in (132 < 300)
        if (grainsPerSilo < upperbound)
        {
            metricValue = fnd.GrainCount == 0 ? 1 : Convert.ToInt16(fnd.GrainCount / upperbound);
        }

        // scale out (605 > 300)
        if (grainsPerSilo >= upperbound)
        {
            metricValue = fnd.SiloCount + 1;
        }

        logger.LogInformation("Grains Per Silo: {GrainsPerSilo}, Upper Bound: {Upperbound}, Grain Count: {GrainCount}, Silo Count: {SiloCount}. Scale to {MetricValue}.",
            grainsPerSilo, upperbound, fnd.GrainCount, fnd.SiloCount, metricValue);

        var response = new GetMetricsResponse();
        response.MetricValues.Add(new MetricValue
        {
            MetricName = MetricName,
            MetricValue_ = metricValue
        });

        return response;
    }

    public override Task<GetMetricSpecResponse> GetMetricSpec(ScaledObjectRef request, ServerCallContext context)
    {
        CheckRequestMetadata(request);

        var response = new GetMetricSpecResponse();
        response.MetricSpecs.Add(new MetricSpec
        {
            MetricName = MetricName,
            TargetSize = 1
        });

        return Task.FromResult(response);
    }

    public override async Task<IsActiveResponse> IsActive(ScaledObjectRef request, ServerCallContext context)
    {
        CheckRequestMetadata(request);

        var result = await AreTooManyGrainsInTheCluster(request);

        logger.LogInformation("Returning IsActive = {IsActive}", request);

        return new IsActiveResponse
        {
            Result = result
        };
    }

    public override async Task StreamIsActive(ScaledObjectRef request, IServerStreamWriter<IsActiveResponse> responseStream, ServerCallContext context)
    {
        CheckRequestMetadata(request);

        while (!context.CancellationToken.IsCancellationRequested)
        {
            if (await AreTooManyGrainsInTheCluster(request))
            {
                logger.LogInformation("Writing IsActiveResopnse to stream with Result = true");

                await responseStream.WriteAsync(new IsActiveResponse
                {
                    Result = true
                });
            }

            await Task.Delay(TimeSpan.FromSeconds(30));
        }
    }

    private static void CheckRequestMetadata(ScaledObjectRef request)
    {
        if (!request.ScalerMetadata.ContainsKey(GrainTypeMetadata)
            || !request.ScalerMetadata.ContainsKey(UpperBoundMetadata)
            || !request.ScalerMetadata.ContainsKey(SiloNameMetadata))
        {
            throw new ArgumentException("graintype, siloNameFilter, and upperbound must be specified");
        }
    }

    private async Task<bool> AreTooManyGrainsInTheCluster(ScaledObjectRef request)
    {
        var grainType = request.ScalerMetadata[GrainTypeMetadata];
        var upperbound = request.ScalerMetadata[UpperBoundMetadata];
        var siloNameFilter = request.ScalerMetadata[SiloNameMetadata];

        var counts = await GetGrainCountInCluster(grainType, siloNameFilter);

        return counts.GrainCount != 0 && counts.SiloCount != 0 && Convert.ToInt32(upperbound) <= (counts.GrainCount / counts.SiloCount);
    }

    private async Task<GrainSaturationSummary> GetGrainCountInCluster(string grainType, string siloNameFilter)
    {
        var statistics = await _managementGrain.GetDetailedGrainStatistics();

        var activeGrainsInCluster = statistics.Select(_ => new GrainInfo(_.GrainType, _.GrainId.Key.ToString()!, _.SiloAddress.ToGatewayUri().AbsoluteUri));
        var activeGrainsOfSpecifiedType = activeGrainsInCluster.Where(_ => _.Type.Contains(grainType, StringComparison.OrdinalIgnoreCase));

        var detailedHosts = await _managementGrain.GetDetailedHosts();

        var silos = detailedHosts.Where(x => x.Status == SiloStatus.Active).Select(_ => new SiloInfo(_.SiloName, _.SiloAddress.ToGatewayUri().AbsoluteUri));
        var activeSiloCount = silos.Where(_ => _.SiloName.Contains(siloNameFilter, StringComparison.OrdinalIgnoreCase)).Count();

        logger.LogInformation("Found {ActiveGrains} instances of {GrainType} in cluster, with {ActiveSiloCount} '{SiloNameFilter}' silos in the cluster hosting {GrainType} grains.",
            activeGrainsOfSpecifiedType.Count(), grainType, activeSiloCount, siloNameFilter, grainType);

        return new GrainSaturationSummary(activeGrainsOfSpecifiedType.Count(), activeSiloCount);
    }
}

public record GrainInfo(string Type, string PrimaryKey, string SiloName);

public record GrainSaturationSummary(long GrainCount, long SiloCount);

public record SiloInfo(string SiloName, string SiloAddress);