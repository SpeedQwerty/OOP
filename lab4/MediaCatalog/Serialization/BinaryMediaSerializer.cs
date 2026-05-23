using System.IO;
using MediaCatalog.Abstractions.Models;
using MediaCatalog.Abstractions.Serialization;

namespace MediaCatalog.Serialization;

/// <summary>
/// Binary file format for the whole catalog (built-in + plugin types).
/// </summary>
public static class BinaryMediaSerializer
{
    public static void SerializeList(List<MediaItem> items, string filePath)
    {
        using var fs = File.Create(filePath);
        using var writer = new BinaryWriter(fs);
        writer.Write(items.Count);
        foreach (var item in items)
            TypeRegistry.Serialize(writer, item);
    }

    public static List<MediaItem> DeserializeList(string filePath)
    {
        using var fs = File.OpenRead(filePath);
        using var reader = new BinaryReader(fs);
        int count = reader.ReadInt32();
        var list = new List<MediaItem>(count);
        for (int i = 0; i < count; i++)
            list.Add(TypeRegistry.Deserialize(reader));
        return list;
    }
}
