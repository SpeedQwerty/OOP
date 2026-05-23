using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using MediaCatalog.Abstractions.Models;
using MediaCatalog.Abstractions.Serialization;
using MediaCatalog.Models;
using MediaCatalog.Plugins;
using MediaCatalog.Serialization;
using Microsoft.Win32;

namespace MediaCatalog.ViewModels;

/// <summary>
/// Main window view model; uses PluginCatalog for add-dialog prototypes (built-in + plugins).
/// </summary>
public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
{
    private MediaItem? _selectedItem;
    private string _status = "Готово";

    public ObservableCollection<MediaItem> Items { get; } = new();

    public MediaItem? SelectedItem
    {
        get => _selectedItem;
        set { _selectedItem = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
    }

    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand ReloadPluginsCommand { get; }

    public MainViewModel()
    {
        AddCommand = new RelayCommand(Add);
        RemoveCommand = new RelayCommand(Remove, () => SelectedItem != null);
        SaveCommand = new RelayCommand(Save);
        LoadCommand = new RelayCommand(Load);
        ReloadPluginsCommand = new RelayCommand(ReloadPlugins);
    }

    private void Add()
    {
        var dialog = new Views.AddItemDialog(PluginCatalog.Prototypes.ToArray());
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
        if (SelectedItem == null) return;
        Items.Remove(SelectedItem);
        SelectedItem = Items.FirstOrDefault();
        Status = "Объект удалён";
    }

    private void Save()
    {
        var dlg = new SaveFileDialog
        {
            Filter = "Бинарные файлы (*.bin)|*.bin|Все файлы (*.*)|*.*",
            DefaultExt = ".bin"
        };
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
        var dlg = new OpenFileDialog
        {
            Filter = "Бинарные файлы (*.bin)|*.bin|Все файлы (*.*)|*.*",
            DefaultExt = ".bin"
        };
        if (dlg.ShowDialog() != true) return;
        try
        {
            var list = BinaryMediaSerializer.DeserializeList(dlg.FileName);
            Items.Clear();
            foreach (var item in list) Items.Add(item);
            SelectedItem = Items.FirstOrDefault();
            Status = $"Загружено {Items.Count} объектов";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            Status = "Ошибка загрузки";
        }
    }

    private void ReloadPlugins()
    {
        var pluginTypes = PluginCatalog.Prototypes
            .Where(p => p.TypeId >= TypeRegistry.PluginTypeIdMin)
            .Select(p => $"{p.TypeName} (id {p.TypeId})")
            .ToList();

        string loadedDlls = PluginLoader.LoadedFileNames.Count > 0
            ? string.Join(Environment.NewLine, PluginLoader.LoadedFileNames)
            : "(нет — проверьте папку Plugins и подпись .plugin.sig)";

        string pluginTypeLines = pluginTypes.Count > 0
            ? string.Join(Environment.NewLine, pluginTypes)
            : "(типы из плагинов не зарегистрированы)";

        MessageBox.Show(
            $"Папка плагинов:{Environment.NewLine}{PluginLoader.PluginsDirectory}{Environment.NewLine}{Environment.NewLine}" +
            $"Загруженные DLL в этом запуске:{Environment.NewLine}{loadedDlls}{Environment.NewLine}{Environment.NewLine}" +
            $"Доп. типы в «Добавить»:{Environment.NewLine}{pluginTypeLines}{Environment.NewLine}{Environment.NewLine}" +
            "Горячая перезагрузка не поддерживается (lab 4): после копирования нового .dll в Plugins полностью закройте и снова запустите приложение.",
            "Плагины", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
}
