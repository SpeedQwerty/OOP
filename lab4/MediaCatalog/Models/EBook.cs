using System.IO;
using MediaCatalog.Abstractions.Models;
using MediaCatalog.Abstractions.Serialization;

namespace MediaCatalog.Models;

public class EBook : MediaItem
{
    public override byte TypeId => TypeRegistry.EBookId;
    public override string TypeName => "Электронная книга";

    private string _author = string.Empty;
    private string _format = "EPUB";
    private long _fileSizeBytes;

    public override MediaItem Clone() => new EBook();
    public string Author { get => _author; set { _author = value; OnPropertyChanged(); } }
    public string Format { get => _format; set { _format = value; OnPropertyChanged(); } }
    public long FileSizeBytes { get => _fileSizeBytes; set { _fileSizeBytes = value; OnPropertyChanged(); } }

    static EBook()
    {
        TypeRegistry.Register<EBook>(TypeRegistry.EBookId, () => new EBook(),
            (w, o) => o.SerializeTo(w), (r, o) => o.DeserializeFrom(r));
    }

    private void SerializeTo(BinaryWriter w)
    {
        w.Write(Title); w.Write(Year); w.Write(Publisher);
        w.Write(Author); w.Write(Format); w.Write(FileSizeBytes);
    }

    private void DeserializeFrom(BinaryReader r)
    {
        Title = r.ReadString(); Year = r.ReadInt32(); Publisher = r.ReadString();
        Author = r.ReadString(); Format = r.ReadString(); FileSizeBytes = r.ReadInt64();
    }
}
