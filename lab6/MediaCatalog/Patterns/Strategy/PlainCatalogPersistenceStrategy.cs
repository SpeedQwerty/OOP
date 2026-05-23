using MediaCatalog.Abstractions.Models;
using MediaCatalog.Serialization;

namespace MediaCatalog.Patterns.Strategy;

/// <summary>
/// Saves and loads raw binary catalog files without an archive pipeline.
/// </summary>
public sealed class PlainCatalogPersistenceStrategy : ICatalogPersistenceStrategy
{
    public void Save(IReadOnlyList<MediaItem> items, string filePath)
    {
        byte[] raw = BinaryMediaSerializer.SerializeToBytes(items.ToList());
        CatalogFilePipeline.WriteFile(filePath, raw, null);
    }

    public IReadOnlyList<MediaItem> Load(string filePath)
    {
        byte[] raw = CatalogFilePipeline.ReadFile(filePath, _ => null);
        return BinaryMediaSerializer.DeserializeFromBytes(raw);
    }
}
