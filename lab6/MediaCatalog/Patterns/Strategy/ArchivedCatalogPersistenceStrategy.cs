using MediaCatalog.Abstractions.Models;
using MediaCatalog.Plugin.Sdk;
using MediaCatalog.Plugins;
using MediaCatalog.Serialization;
using MediaCatalog.Services;

namespace MediaCatalog.Patterns.Strategy;

/// <summary>
/// Saves and loads catalog files using the active archive pipeline plugin from settings.
/// </summary>
public sealed class ArchivedCatalogPersistenceStrategy : ICatalogPersistenceStrategy
{
    public void Save(IReadOnlyList<MediaItem> items, string filePath)
    {
        PipelineService.ReloadSettings();
        PipelineService.ApplySettingsToAllPlugins();
        IArchivePipelinePlugin? pipeline = PipelineService.GetActivePlugin();
        byte[] raw = BinaryMediaSerializer.SerializeToBytes(items.ToList());
        CatalogFilePipeline.WriteFile(filePath, raw, pipeline);
    }

    public IReadOnlyList<MediaItem> Load(string filePath)
    {
        PipelineService.ReloadSettings();
        PipelineService.ApplySettingsToAllPlugins();
        byte[] raw = CatalogFilePipeline.ReadFile(filePath, PipelinePluginRegistry.TryGet);
        return BinaryMediaSerializer.DeserializeFromBytes(raw);
    }
}
