using System.IO;
using MediaCatalog.Abstractions.Models;
using MediaCatalog.Abstractions.Serialization;

namespace MediaCatalog.Models;

public class Magazine : MediaItem
{
    public override byte TypeId => TypeRegistry.MagazineId;
    public override string TypeName => "Журнал";

    private int _issueNumber;
    private string _theme = string.Empty;
    private int _periodicityMonths;

    public override MediaItem Clone() => new Magazine();
    public int IssueNumber { get => _issueNumber; set { _issueNumber = value; OnPropertyChanged(); } }
    public string Theme { get => _theme; set { _theme = value; OnPropertyChanged(); } }
    public int PeriodicityMonths { get => _periodicityMonths; set { _periodicityMonths = value; OnPropertyChanged(); } }

    static Magazine()
    {
        TypeRegistry.Register<Magazine>(TypeRegistry.MagazineId, () => new Magazine(),
            (w, o) => o.SerializeTo(w), (r, o) => o.DeserializeFrom(r));
    }

    private void SerializeTo(BinaryWriter w)
    {
        w.Write(Title); w.Write(Year); w.Write(Publisher);
        w.Write(IssueNumber); w.Write(Theme); w.Write(PeriodicityMonths);
    }

    private void DeserializeFrom(BinaryReader r)
    {
        Title = r.ReadString(); Year = r.ReadInt32(); Publisher = r.ReadString();
        IssueNumber = r.ReadInt32(); Theme = r.ReadString(); PeriodicityMonths = r.ReadInt32();
    }
}
