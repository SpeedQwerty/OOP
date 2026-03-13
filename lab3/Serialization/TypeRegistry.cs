using System.IO;

namespace MediaCatalog.Serialization;

/// <summary>
/// Реестр типов для полиморфной бинарной сериализации без if-else/switch.
/// Новые классы регистрируют себя в статическом конструкторе.
/// </summary>
public static class TypeRegistry
{
    public const byte BookId = 1;
    public const byte MagazineId = 2;
    public const byte EBookId = 3;
    public const byte MovieId = 4;
    public const byte MusicAlbumId = 5;
    public const byte AudiobookId = 6;

    private static readonly Dictionary<byte, TypeEntry> _byId = new();

    public static void Register<T>(byte typeId, Func<T> creator,
        Action<BinaryWriter, T> serializer, Action<BinaryReader, T> deserializer)
        where T : Models.MediaItem
    {
        _byId[typeId] = new TypeEntry(
            creator,
            (w, o) => serializer(w, (T)o),
            (r, o) => deserializer(r, (T)o));
    }

    public static Models.MediaItem Create(byte typeId)
    {
        if (_byId.TryGetValue(typeId, out var entry))
            return entry.Creator();
        throw new InvalidOperationException($"Неизвестный тип: {typeId}");
    }

    public static void Serialize(BinaryWriter w, Models.MediaItem item)
    {
        if (_byId.TryGetValue(item.TypeId, out var entry))
        {
            w.Write(item.TypeId);
            entry.Serializer(w, item);
            return;
        }
        throw new InvalidOperationException($"Неизвестный тип: {item.TypeId}");
    }

    public static Models.MediaItem Deserialize(BinaryReader r)
    {
        byte typeId = r.ReadByte();
        if (_byId.TryGetValue(typeId, out var entry))
        {
            var item = entry.Creator();
            entry.Deserializer(r, item);
            return item;
        }
        throw new InvalidOperationException($"Неизвестный тип: {typeId}");
    }

    private record TypeEntry(
        Func<Models.MediaItem> Creator,
        Action<BinaryWriter, Models.MediaItem> Serializer,
        Action<BinaryReader, Models.MediaItem> Deserializer);
}
