namespace Orleans.Runtime;

[Serializable]
public class RegionBasedPlacement : PlacementStrategy
{
    /// <summary>
    /// Gets the singleton instance of this class.
    /// </summary>
    internal static RegionBasedPlacement Singleton { get; } = new RegionBasedPlacement();
}
