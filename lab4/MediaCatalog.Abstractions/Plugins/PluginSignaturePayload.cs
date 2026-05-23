namespace MediaCatalog.Abstractions.Plugins;

/// <summary>
/// Signed metadata stored next to a plugin assembly (.plugin.sig file).
/// Covers integrity (hash) and activation window (not-before / not-after).
/// </summary>
public sealed class PluginSignaturePayload
{
    public const string FileExtension = ".plugin.sig";
    public const string Magic = "MCP1";

    /// <summary>UTC moment when the plugin becomes valid.</summary>
    public DateTime NotBeforeUtc { get; init; }

    /// <summary>UTC moment when the plugin expires.</summary>
    public DateTime NotAfterUtc { get; init; }

    /// <summary>SHA-256 hash of the plugin DLL bytes.</summary>
    public byte[] AssemblyHash { get; init; } = Array.Empty<byte>();

    /// <summary>RSA-PSS signature over canonical payload bytes.</summary>
    public byte[] Signature { get; init; } = Array.Empty<byte>();
}
