using System.IO;
using MediaCatalog.Abstractions.Models;
using MediaCatalog.Abstractions.Serialization;

namespace MediaCatalog.Models;

/// <summary>Printed book — built-in leaf of the media hierarchy.</summary>
public class Book : MediaItem
{
    public override byte TypeId => TypeRegistry.BookId;
    public override string TypeName => "Книга";

    private string _author = string.Empty;
    private int _pageCount;
    private string _isbn = string.Empty;

    public override MediaItem Clone() => new Book();

    public string Author { get => _author; set { _author = value; OnPropertyChanged(); } }
    public int PageCount { get => _pageCount; set { _pageCount = value; OnPropertyChanged(); } }
    public string ISBN { get => _isbn; set { _isbn = value; OnPropertyChanged(); } }

    static Book()
    {
        TypeRegistry.Register<Book>(TypeRegistry.BookId, () => new Book(),
            (w, o) => o.SerializeTo(w), (r, o) => o.DeserializeFrom(r));
    }

    private void SerializeTo(BinaryWriter w)
    {
        w.Write(Title); w.Write(Year); w.Write(Publisher);
        w.Write(Author); w.Write(PageCount); w.Write(ISBN);
    }

    private void DeserializeFrom(BinaryReader r)
    {
        Title = r.ReadString(); Year = r.ReadInt32(); Publisher = r.ReadString();
        Author = r.ReadString(); PageCount = r.ReadInt32(); ISBN = r.ReadString();
    }
}
