using MediaCatalog.Plugin.Sdk;

namespace MediaCatalog.Plugins;

/// <summary>
/// Holds archive pipeline plugins discovered at runtime (variant 2 — archiving).
/// </summary>
public static class PipelinePluginRegistry
{
    private static readonly Dictionary<string, IArchivePipelinePlugin> _byId = new(StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyCollection<IArchivePipelinePlugin> All => _byId.Values;

    /// <summary>Registers or replaces a pipeline plugin by <see cref="IArchivePipelinePlugin.PipelineId"/>.</summary>
    public static void Register(IArchivePipelinePlugin plugin)
    {
        _byId[plugin.PipelineId] = plugin;
    }

    public static IArchivePipelinePlugin? TryGet(string pipelineId) =>
        _byId.TryGetValue(pipelineId, out var plugin) ? plugin : null;

    public static void Clear() => _byId.Clear();
}
