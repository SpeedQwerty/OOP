using System.ComponentModel;

namespace MediaCatalog.Models;

/// <summary>
/// Абстрактный базовый класс для медиа-контента в каталоге.
/// </summary>
public abstract class MediaItem : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private int _year;
    private string _publisher = string.Empty;

    public string Title { get => _title; set { _title = value; OnPropertyChanged(); } }
    public int Year { get => _year; set { _year = value; OnPropertyChanged(); } }
    public string Publisher { get => _publisher; set { _publisher = value; OnPropertyChanged(); } }

    public abstract byte TypeId { get; }
    public abstract string TypeName { get; }
    public abstract MediaItem Clone();

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public override string ToString() => $"{TypeName}: {Title} ({Year})";
}
