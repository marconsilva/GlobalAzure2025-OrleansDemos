namespace Orleans.Placement;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class RegionBasedPlacementAttribute : PlacementAttribute
{
    public RegionBasedPlacementAttribute() : base(RegionBasedPlacement.Singleton) { }
}

