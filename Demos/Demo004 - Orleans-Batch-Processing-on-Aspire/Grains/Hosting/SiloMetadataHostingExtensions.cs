using Microsoft.Extensions.Configuration;
using Orleans.Placement;
using Orleans.Runtime.MembershipService.SiloMetadata;
using Orleans.Runtime.Placement;

namespace Orleans.Runtime;

public static class SiloMetadataHostingExtensions
{
    public static ISiloBuilder UseSiloMetadataWithRegion(this ISiloBuilder builder, string regionScope, IConfigurationSection? configurationSection = null)
    {
        var dictionary = configurationSection?.Get<Dictionary<string, string>>() ?? [];
        dictionary.TryAdd(RegionBasedPlacementDirector.RegionHintKey, regionScope);

        builder.UseSiloMetadata(dictionary);
        builder.ConfigureServices(services => services.AddPlacementDirector<RegionBasedPlacement, RegionBasedPlacementDirector>());
        return builder;
    }
}
