using Orleans.Runtime.MembershipService.SiloMetadata;

namespace Orleans.Runtime.Placement;

internal class RegionBasedPlacementDirector(ISiloMetadataCache siloMetadataCache) : IPlacementDirector
{
    public const string RegionHintKey = "cloud.region";

    public Task<SiloAddress> OnAddActivation(PlacementStrategy strategy, PlacementTarget target, IPlacementContext context)
    {
        var regionScope = target.RequestContextData[RegionHintKey] as string ?? target.GrainIdentity.GetKeyExtension();

        var compatibleSilos = context.GetCompatibleSilos(target)
            .Where(s => siloMetadataCache.GetSiloMetadata(s).Metadata.GetValueOrDefault(RegionHintKey) == regionScope)
            .ToArray();

        if (compatibleSilos.Length == 0)
        {
            throw new SiloUnavailableException($"Cannot place grain '{target.GrainIdentity}' because there are no compatible silos in region {regionScope}");
        }

        // If a valid placement hint was specified, use it.
        return Task.FromResult(IPlacementDirector.GetPlacementHint(target.RequestContextData, compatibleSilos) is { } placementHint
            ? placementHint : compatibleSilos[Random.Shared.Next(compatibleSilos.Length)]);
    }
}
