using System.IO;
using MediaCatalog.Abstractions.Models;
using MediaCatalog.Abstractions.Serialization;

namespace MediaCatalog.Plugin.Podcast.Models;

/// <summary>
/// Plugin-provided media type: podcast entry in the catalog hierarchy.
/// </summary>
public sealed class Podcast : MediaItem
{
    public const byte TypeIdValue = 100;

    public override byte TypeId => TypeIdValue;
    public override string TypeName => "Подкаст";

    private string _host = string.Empty;
    private int _episodeCount;
    private int _avgEpisodeMinutes;

    public string Host
    {
        get => _host;
        set { _host = value; OnPropertyChanged(); }
    }

    public int EpisodeCount
    {
        get => _episodeCount;
        set { _episodeCount = value; OnPropertyChanged(); }
    }

    public int AvgEpisodeMinutes
    {
        get => _avgEpisodeMinutes;
        set { _avgEpisodeMinutes = value; OnPropertyChanged(); }
    }

    public override MediaItem Clone() => new Podcast();

    static Podcast()
    {
        TypeRegistry.Register<Podcast>(
            TypeIdValue,
            () => new Podcast(),
            (w, o) => o.SerializeTo(w),
            (r, o) => o.DeserializeFrom(r));
    }

    private void SerializeTo(BinaryWriter writer)
    {
        writer.Write(Title);
        writer.Write(Year);
        writer.Write(Publisher);
        writer.Write(Host);
        writer.Write(EpisodeCount);
        writer.Write(AvgEpisodeMinutes);
    }

    private void DeserializeFrom(BinaryReader reader)
    {
        Title = reader.ReadString();
        Year = reader.ReadInt32();
        Publisher = reader.ReadString();
        Host = reader.ReadString();
        EpisodeCount = reader.ReadInt32();
        AvgEpisodeMinutes = reader.ReadInt32();
    }
}
