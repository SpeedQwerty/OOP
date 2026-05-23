using System.ComponentModel;

namespace MediaCatalog.Abstractions.Models;

/// <summary>
/// Abstract base class for all media items in the catalog hierarchy.
/// Plugins extend this class to add new media types without modifying the host.
/// </summary>
public abstract class MediaItem : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private int _year;
    private string _publisher = string.Empty;

    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); }
    }

    public int Year
    {
        get => _year;
        set { _year = value; OnPropertyChanged(); }
    }

    public string Publisher
    {
        get => _publisher;
        set { _publisher = value; OnPropertyChanged(); }
    }

    /// <summary>Unique binary serialization identifier for this type.</summary>
    public abstract byte TypeId { get; }

    /// <summary>Human-readable type name shown in the UI.</summary>
    public abstract string TypeName { get; }

    /// <summary>Creates an empty instance of the same media type for the add dialog.</summary>
    public abstract MediaItem Clone();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public override string ToString() => $"{TypeName}: {Title} ({Year})";
}
