using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using MediaCatalog.Models;
using MediaCatalog.Serialization;
using Microsoft.Win32;

namespace MediaCatalog.ViewModels;

public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
{
    private MediaItem? _selectedItem;
    private string _status = "Готово";

    public ObservableCollection<MediaItem> Items { get; } = new();
    public MediaItem? SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged(); System.Windows.Input.CommandManager.InvalidateRequerySuggested(); } }
    public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }

    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }

    private static readonly MediaItem[] SampleCreators =
    {
        new Book(),
        new Magazine(),
        new EBook(),
        new Movie(),
        new MusicAlbum(),
        new Audiobook()
    };

    public MainViewModel()
    {
        AddCommand = new RelayCommand(Add);
        RemoveCommand = new RelayCommand(Remove, () => SelectedItem != null);
        SaveCommand = new RelayCommand(Save);
        LoadCommand = new RelayCommand(Load);
    }

    private void Add()
    {
        var dialog = new Views.AddItemDialog(SampleCreators);
        if (dialog.ShowDialog() == true && dialog.SelectedType != null)
        {
            var copy = dialog.SelectedType.Clone();
            Items.Add(copy);
            SelectedItem = copy;
            Status = $"Добавлен: {copy.TypeName}";
        }
    }

    private void Remove()
    {
        if (SelectedItem != null)
        {
            Items.Remove(SelectedItem);
            SelectedItem = Items.FirstOrDefault();
            Status = "Объект удалён";
        }
    }

    private void Save()
    {
        var dlg = new SaveFileDialog { Filter = "Бинарные файлы (*.bin)|*.bin|Все файлы (*.*)|*.*", DefaultExt = ".bin" };
        if (dlg.ShowDialog() != true) return;
        try
        {
            BinaryMediaSerializer.SerializeList(Items.ToList(), dlg.FileName);
            Status = $"Сохранено в {Path.GetFileName(dlg.FileName)}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            Status = "Ошибка сохранения";
        }
    }

    private void Load()
    {
        var dlg = new OpenFileDialog { Filter = "Бинарные файлы (*.bin)|*.bin|Все файлы (*.*)|*.*", DefaultExt = ".bin" };
        if (dlg.ShowDialog() != true) return;
        try
        {
            var list = BinaryMediaSerializer.DeserializeList(dlg.FileName);
            Items.Clear();
            foreach (var item in list) Items.Add(item);
            SelectedItem = Items.FirstOrDefault();
            Status = $"Загружено {Items.Count} объектов из {Path.GetFileName(dlg.FileName)}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            Status = "Ошибка загрузки";
        }
    }

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
}
