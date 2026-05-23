namespace MediaCatalog.Abstractions.Plugins;

/// <summary>
/// Loads the RSA public key used by the host to verify plugin signatures.
/// </summary>
public static class PluginTrustKeys
{
    private static string? _cachedPublicKeyXml;

    /// <summary>
    /// Returns the trusted public key XML (from file or embedded fallback path).
    /// </summary>
    public static string GetPublicKeyXml(string? baseDirectory = null)
    {
        if (_cachedPublicKeyXml != null)
            return _cachedPublicKeyXml;

        string root = baseDirectory ?? AppContext.BaseDirectory;
        string keyPath = Path.Combine(root, "Keys", "plugin-trust.public.xml");
        if (!File.Exists(keyPath))
            throw new FileNotFoundException($"Plugin trust public key not found: {keyPath}");

        _cachedPublicKeyXml = File.ReadAllText(keyPath);
        return _cachedPublicKeyXml;
    }
}
