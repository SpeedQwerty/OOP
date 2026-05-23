namespace FriendPlugin.LegacyPack;

/// <summary>
/// Foreign API from a classmate's plugin assembly (not <c>IArchivePipelinePlugin</c>).
/// The host integrates it only through an Adapter plugin.
/// </summary>
public interface ILegacyPackService
{
    /// <summary>Vendor display name shown in their own tooling.</summary>
    string VendorName { get; }

    /// <summary>Packs catalog bytes using the classmate-specific container format.</summary>
    byte[] PackData(byte[] plainBytes, LegacyPackOptions options);

    /// <summary>Unpacks bytes produced by <see cref="PackData"/>.</summary>
    byte[] UnpackData(byte[] packedBytes);
}
