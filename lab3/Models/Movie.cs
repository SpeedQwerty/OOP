using System.IO;
using MediaCatalog.Serialization;

namespace MediaCatalog.Models;

public class Movie : MediaItem
{
    public override byte TypeId => TypeRegistry.MovieId;
    public override string TypeName => "Фильм";

    private string _director = string.Empty;
    private int _durationMinutes;
    private string _genre = string.Empty;

    public override MediaItem Clone() => new Movie();
    public string Director { get => _director; set { _director = value; OnPropertyChanged(); } }
    public int DurationMinutes { get => _durationMinutes; set { _durationMinutes = value; OnPropertyChanged(); } }
    public string Genre { get => _genre; set { _genre = value; OnPropertyChanged(); } }

    static Movie()
    {
        TypeRegistry.Register<Movie>(TypeRegistry.MovieId, () => new Movie(), (w, o) => o.SerializeTo(w), (r, o) => o.DeserializeFrom(r));
    }

    private void SerializeTo(BinaryWriter w)
    {
        w.Write(Title); w.Write(Year); w.Write(Publisher);
        w.Write(Director); w.Write(DurationMinutes); w.Write(Genre);
    }

    private void DeserializeFrom(BinaryReader r)
    {
        Title = r.ReadString(); Year = r.ReadInt32(); Publisher = r.ReadString();
        Director = r.ReadString(); DurationMinutes = r.ReadInt32(); Genre = r.ReadString();
    }
}
