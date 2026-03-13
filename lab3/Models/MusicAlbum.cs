using System.IO;
using MediaCatalog.Serialization;

namespace MediaCatalog.Models;

public class MusicAlbum : MediaItem
{
    public override byte TypeId => TypeRegistry.MusicAlbumId;
    public override string TypeName => "Музыкальный альбом";

    private string _artist = string.Empty;
    private int _trackCount;
    private string _genre = string.Empty;

    public override MediaItem Clone() => new MusicAlbum();
    public string Artist { get => _artist; set { _artist = value; OnPropertyChanged(); } }
    public int TrackCount { get => _trackCount; set { _trackCount = value; OnPropertyChanged(); } }
    public string Genre { get => _genre; set { _genre = value; OnPropertyChanged(); } }

    static MusicAlbum()
    {
        TypeRegistry.Register<MusicAlbum>(TypeRegistry.MusicAlbumId, () => new MusicAlbum(), (w, o) => o.SerializeTo(w), (r, o) => o.DeserializeFrom(r));
    }

    private void SerializeTo(BinaryWriter w)
    {
        w.Write(Title); w.Write(Year); w.Write(Publisher);
        w.Write(Artist); w.Write(TrackCount); w.Write(Genre);
    }

    private void DeserializeFrom(BinaryReader r)
    {
        Title = r.ReadString(); Year = r.ReadInt32(); Publisher = r.ReadString();
        Artist = r.ReadString(); TrackCount = r.ReadInt32(); Genre = r.ReadString();
    }
}
