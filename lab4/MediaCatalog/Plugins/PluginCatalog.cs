using MediaCatalog.Abstractions.Models;

namespace MediaCatalog.Plugins;

/// <summary>
/// Central catalog of media type prototypes (built-in + loaded plugins).
/// </summary>
public static class PluginCatalog
{
    private static readonly List<MediaItem> _prototypes = new();

    public static IReadOnlyList<MediaItem> Prototypes => _prototypes;

    /// <summary>Adds built-in prototypes before plugin loading.</summary>
    public static void RegisterBuiltIn(IEnumerable<MediaItem> items)
    {
        _prototypes.Clear();
        _prototypes.AddRange(items);
    }

    /// <summary>Called by the host when a plugin registers a new type.</summary>
    public static void RegisterFromPlugin(MediaItem prototype)
    {
        if (_prototypes.Any(p => p.TypeId == prototype.TypeId))
            throw new InvalidOperationException($"Type id {prototype.TypeId} is already in the catalog.");

        _prototypes.Add(prototype);
    }
}
