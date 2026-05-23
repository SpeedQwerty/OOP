using System.IO;
using MediaCatalog.Abstractions.Models;

namespace MediaCatalog.Abstractions.Serialization;

/// <summary>
/// Registry for polymorphic binary serialization without switch/if chains.
/// Built-in types and plugins register themselves via static constructors or plugin initialization.
/// </summary>
public static class TypeRegistry
{
    // Built-in type identifiers (lab3 core hierarchy).
    public const byte BookId = 1;
    public const byte MagazineId = 2;
    public const byte EBookId = 3;
    public const byte MovieId = 4;
    public const byte MusicAlbumId = 5;
    public const byte AudiobookId = 6;

    /// <summary>First type id reserved for dynamically loaded plugins.</summary>
    public const byte PluginTypeIdMin = 100;

    private static readonly Dictionary<byte, TypeEntry> _byId = new();

    /// <summary>
    /// Registers a media type for creation and binary serialization.
    /// </summary>
    public static void Register<T>(byte typeId, Func<T> creator,
        Action<BinaryWriter, T> serializer, Action<BinaryReader, T> deserializer)
        where T : MediaItem
    {
        if (_byId.ContainsKey(typeId))
            throw new InvalidOperationException($"Type id {typeId} is already registered.");

        _byId[typeId] = new TypeEntry(
            creator,
            (w, o) => serializer(w, (T)o),
            (r, o) => deserializer(r, (T)o));
    }

    public static MediaItem Create(byte typeId)
    {
        if (_byId.TryGetValue(typeId, out var entry))
            return entry.Creator();

        throw new InvalidOperationException($"Unknown media type id: {typeId}");
    }

    public static void Serialize(BinaryWriter writer, MediaItem item)
    {
        if (_byId.TryGetValue(item.TypeId, out var entry))
        {
            writer.Write(item.TypeId);
            entry.Serializer(writer, item);
            return;
        }

        throw new InvalidOperationException($"Unknown media type id: {item.TypeId}");
    }

    public static MediaItem Deserialize(BinaryReader reader)
    {
        byte typeId = reader.ReadByte();
        if (_byId.TryGetValue(typeId, out var entry))
        {
            var item = entry.Creator();
            entry.Deserializer(reader, item);
            return item;
        }

        throw new InvalidOperationException($"Unknown media type id: {typeId}");
    }

    public static IReadOnlyCollection<byte> RegisteredTypeIds => _byId.Keys;

    private record TypeEntry(
        Func<MediaItem> Creator,
        Action<BinaryWriter, MediaItem> Serializer,
        Action<BinaryReader, MediaItem> Deserializer);
}
