using System.IO;
using MediaCatalog.Abstractions.Models;
using MediaCatalog.Abstractions.Serialization;

namespace MediaCatalog.Models;

public class Audiobook : MediaItem
{
    public override byte TypeId => TypeRegistry.AudiobookId;
    public override string TypeName => "Аудиокнига";

    private string _author = string.Empty;
    private string _narrator = string.Empty;
    private int _durationMinutes;

    public override MediaItem Clone() => new Audiobook();
    public string Author { get => _author; set { _author = value; OnPropertyChanged(); } }
    public string Narrator { get => _narrator; set { _narrator = value; OnPropertyChanged(); } }
    public int DurationMinutes { get => _durationMinutes; set { _durationMinutes = value; OnPropertyChanged(); } }

    static Audiobook()
    {
        TypeRegistry.Register<Audiobook>(TypeRegistry.AudiobookId, () => new Audiobook(),
            (w, o) => o.SerializeTo(w), (r, o) => o.DeserializeFrom(r));
    }

    private void SerializeTo(BinaryWriter w)
    {
        w.Write(Title); w.Write(Year); w.Write(Publisher);
        w.Write(Author); w.Write(Narrator); w.Write(DurationMinutes);
    }

    private void DeserializeFrom(BinaryReader r)
    {
        Title = r.ReadString(); Year = r.ReadInt32(); Publisher = r.ReadString();
        Author = r.ReadString(); Narrator = r.ReadString(); DurationMinutes = r.ReadInt32();
    }
}
