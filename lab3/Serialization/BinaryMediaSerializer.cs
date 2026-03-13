using System.IO;
using MediaCatalog.Models;

namespace MediaCatalog.Serialization;

public static class BinaryMediaSerializer
{
    public static void SerializeList(List<MediaItem> items, string filePath)
    {
        using var fs = File.Create(filePath);
        using var bw = new BinaryWriter(fs);
        bw.Write(items.Count);
        foreach (var item in items)
            TypeRegistry.Serialize(bw, item);
    }

    public static List<MediaItem> DeserializeList(string filePath)
    {
        using var fs = File.OpenRead(filePath);
        using var br = new BinaryReader(fs);
        var count = br.ReadInt32();
        var list = new List<MediaItem>(count);
        for (int i = 0; i < count; i++)
            list.Add(TypeRegistry.Deserialize(br));
        return list;
    }
}
