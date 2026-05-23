using MediaCatalog.Abstractions.Models;
using MediaCatalog.Patterns.Strategy;
using MediaCatalog.Services;

namespace MediaCatalog.Services;

/// <summary>
/// Facade pattern: single entry point for save/load that hides serialization, pipeline,
/// file header handling, and strategy selection from the UI layer.
/// </summary>
public sealed class CatalogPersistenceFacade
{
    private readonly PlainCatalogPersistenceStrategy _plain = new();
    private readonly ArchivedCatalogPersistenceStrategy _archived = new();

    /// <summary>Persists items using plain or archived strategy based on current settings.</summary>
    public void Save(IReadOnlyList<MediaItem> items, string filePath)
    {
        ICatalogPersistenceStrategy strategy = SelectStrategy();
        strategy.Save(items, filePath);
    }

    /// <summary>Loads items using plain or archived strategy based on file header and settings.</summary>
    public IReadOnlyList<MediaItem> Load(string filePath)
    {
        ICatalogPersistenceStrategy strategy = SelectStrategy();
        return strategy.Load(filePath);
    }

    /// <summary>Returns a short label describing the strategy used for the status line.</summary>
    public string GetActiveModeLabel()
    {
        if (!PipelineService.Settings.PipelineEnabled)
            return "без архивации";

        var plugin = PipelineService.GetActivePlugin();
        return plugin != null ? $"архив: {plugin.DisplayName}" : "без архивации";
    }

    private ICatalogPersistenceStrategy SelectStrategy() =>
        PipelineService.Settings.PipelineEnabled && PipelineService.GetActivePlugin() != null
            ? _archived
            : _plain;
}
