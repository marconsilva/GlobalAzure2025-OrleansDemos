namespace Orleans.Placement;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class RegionDelegatingPlacementAttribute<T> : PlacementAttribute where T : PlacementStrategy, new()
{
    public RegionDelegatingPlacementAttribute() : base(new RegionDelegatingPlacement(new T())) { }
}

