using System.Collections.Immutable;
using Orleans.Runtime.MembershipService.SiloMetadata;

namespace Orleans.Runtime.Placement;

internal class RegionDelegatingPlacementDirector(ISiloMetadataCache siloMetadataCache, PlacementDirectorResolver directorResolver) : IPlacementDirector
{
    public Task<SiloAddress> OnAddActivation(PlacementStrategy strategy, PlacementTarget target, IPlacementContext context)
    {
        var innerStrategy = ((strategy as RegionDelegatingPlacement) ?? throw new ArgumentException("Argument must be of type RegionBasedPlacement", nameof(strategy))).InnerStrategy;
        var placementDirector = directorResolver.GetPlacementDirector(innerStrategy);

        var regionScope = target.RequestContextData?.TryGetValue(RegionDelegatingPlacement.RegionHintKey, out var regionHint) == true && regionHint is string
            ? (string)regionHint : target.GrainIdentity.GetKeyExtension();

        context = new RegionalPlacementContext(context, siloMetadataCache, regionScope);
        return placementDirector.OnAddActivation(innerStrategy, target, context);
    }

    private class RegionalPlacementContext(IPlacementContext context, ISiloMetadataCache siloMetadataCache, string? regionScope) : IPlacementContext
    {
        public SiloAddress LocalSilo => context.LocalSilo;

        public SiloStatus LocalSiloStatus => context.LocalSiloStatus;

        public SiloAddress[] GetCompatibleSilos(PlacementTarget target) => FilterRegionCompatibleSilos(context.GetCompatibleSilos(target));

        public IReadOnlyDictionary<ushort, SiloAddress[]> GetCompatibleSilosWithVersions(PlacementTarget target) => context.GetCompatibleSilosWithVersions(target)
            .Select(kvp => new KeyValuePair<ushort, SiloAddress[]>(kvp.Key, FilterRegionCompatibleSilos(kvp.Value))).ToImmutableDictionary();

        private SiloAddress[] FilterRegionCompatibleSilos(SiloAddress[] addresses) => [.. addresses.Where(s =>
            siloMetadataCache.GetSiloMetadata(s).Metadata.GetValueOrDefault(RegionDelegatingPlacement.RegionHintKey) == regionScope)];
    }
}
