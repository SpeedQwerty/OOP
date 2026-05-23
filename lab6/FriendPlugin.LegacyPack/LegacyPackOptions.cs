namespace FriendPlugin.LegacyPack;

/// <summary>
/// Options type used by the classmate plugin (incompatible with the host pipeline settings model).
/// </summary>
public sealed class LegacyPackOptions
{
    /// <summary>Compression strength from 1 (fast) to 9 (strong).</summary>
    public int Strength { get; set; } = 5;

    /// <summary>When true, prepends a vendor-specific file marker.</summary>
    public bool AddVendorHeader { get; set; } = true;
}
