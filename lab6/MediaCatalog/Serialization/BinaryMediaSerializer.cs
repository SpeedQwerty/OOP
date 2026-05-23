using System.IO;
using MediaCatalog.Abstractions.Models;
using MediaCatalog.Abstractions.Serialization;

namespace MediaCatalog.Serialization;

/// <summary>
/// Binary file format for the whole catalog (built-in + plugin types).
/// </summary>
public static class BinaryMediaSerializer
{
    /// <summary>Serializes the catalog to a byte array (before optional archive pipeline).</summary>
    public static byte[] SerializeToBytes(List<MediaItem> items)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(items.Count);
        foreach (var item in items)
            TypeRegistry.Serialize(writer, item);
        return ms.ToArray();
    }

    /// <summary>Deserializes catalog bytes (after optional archive pipeline decompression).</summary>
    public static List<MediaItem> DeserializeFromBytes(byte[] catalogBytes)
    {
        using var ms = new MemoryStream(catalogBytes);
        using var reader = new BinaryReader(ms);
        int count = reader.ReadInt32();
        var list = new List<MediaItem>(count);
        for (int i = 0; i < count; i++)
            list.Add(TypeRegistry.Deserialize(reader));
        return list;
    }

    public static void SerializeList(List<MediaItem> items, string filePath)
    {
        File.WriteAllBytes(filePath, SerializeToBytes(items));
    }

    public static List<MediaItem> DeserializeList(string filePath)
    {
        return DeserializeFromBytes(File.ReadAllBytes(filePath));
    }
}
