using Orleans.Metadata;

namespace Orleans.Runtime;

[Serializable]
public class RegionDelegatingPlacement(PlacementStrategy delegatedStrategy) : PlacementStrategy
{
    public const string RegionHintKey = "cloud.region";

    private const string PlacementStrategyProperty = "delegated-placement-strategy";

    private static readonly PlacementStrategy DefaultStrategy = new RandomPlacement();

    public RegionDelegatingPlacement() : this(DefaultStrategy) { }

    internal PlacementStrategy InnerStrategy { get; private set; } = delegatedStrategy;

    public override void Initialize(GrainProperties properties)
    {
        base.Initialize(properties);

        if (properties.Properties.TryGetValue(PlacementStrategyProperty, out var placementStrategy))
        {
            InnerStrategy = (Type.GetType(placementStrategy) is { } strategyType && Activator.CreateInstance(strategyType) is PlacementStrategy strategyInstance)
                ? strategyInstance : throw new InvalidOperationException($"Unable to create instance of placement strategy {placementStrategy}");
        }
    }

    public override void PopulateGrainProperties(IServiceProvider services, Type grainClass, GrainType grainType, Dictionary<string, string> properties)
    {
        properties.Add(PlacementStrategyProperty, InnerStrategy.GetType().AssemblyQualifiedName!);

        base.PopulateGrainProperties(services, grainClass, grainType, properties);
    }
}
