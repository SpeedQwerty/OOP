using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using MediaCatalog.Abstractions.Models;
using MediaCatalog.Abstractions.Serialization;
using MediaCatalog.Plugin.Sdk;
using MediaCatalog.Patterns.Observer;
using MediaCatalog.Patterns.Strategy;
using MediaCatalog.Plugins;
using MediaCatalog.Services;
using Microsoft.Win32;

namespace MediaCatalog.ViewModels;

/// <summary>
/// Main window view model. Uses Facade (persistence), Strategy (item creation), Observer (status events).
/// </summary>
public class MainViewModel : System.ComponentModel.INotifyPropertyChanged, ICatalogObserver
{
    private readonly CatalogPersistenceFacade _persistence = new();
    private readonly CatalogSubject _catalogSubject = new();
    private readonly MediaItemCreationContext _itemCreation = new(new ClonePrototypeStrategy());

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
    public ICommand OpenSettingsCommand { get; }
    public ICommand ShowPluginsInfoCommand { get; }

    public MainViewModel()
    {
        _catalogSubject.Attach(this);

        AddCommand = new RelayCommand(Add);
        RemoveCommand = new RelayCommand(Remove, () => SelectedItem != null);
        SaveCommand = new RelayCommand(Save);
        LoadCommand = new RelayCommand(Load);
        OpenSettingsCommand = new RelayCommand(OpenSettings);
        ShowPluginsInfoCommand = new RelayCommand(ShowPluginsInfo);
    }

    /// <inheritdoc />
    public void OnCatalogEvent(CatalogEventArgs args) => Status = args.Message;

    private void Add()
    {
        var dialog = new Views.AddItemDialog(PluginCatalog.Prototypes.ToArray());
        if (dialog.ShowDialog() == true && dialog.SelectedType != null)
        {
            var copy = _itemCreation.CreateItem(dialog.SelectedType);
            Items.Add(copy);
            SelectedItem = copy;
            _catalogSubject.Notify(CatalogEventKind.ItemAdded, $"Добавлен: {copy.TypeName}");
        }
    }

    private void Remove()
    {
        if (SelectedItem == null) return;
        Items.Remove(SelectedItem);
        SelectedItem = Items.FirstOrDefault();
        _catalogSubject.Notify(CatalogEventKind.ItemRemoved, "Объект удалён");
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
            _persistence.Save(Items.ToList(), dlg.FileName);
            string mode = _persistence.GetActiveModeLabel();
            _catalogSubject.Notify(CatalogEventKind.Saved,
                $"Сохранено ({mode}) — {Path.GetFileName(dlg.FileName)}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            _catalogSubject.Notify(CatalogEventKind.Status, "Ошибка сохранения");
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
            var list = _persistence.Load(dlg.FileName);
            Items.Clear();
            foreach (var item in list) Items.Add(item);
            SelectedItem = Items.FirstOrDefault();
            _catalogSubject.Notify(CatalogEventKind.Loaded, $"Загружено {Items.Count} объектов");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            _catalogSubject.Notify(CatalogEventKind.Status, "Ошибка загрузки");
        }
    }

    private void OpenSettings()
    {
        var window = new Views.SettingsWindow { Owner = Application.Current.MainWindow };
        if (window.ShowDialog() == true)
            _catalogSubject.Notify(CatalogEventKind.Status, "Настройки плагинов сохранены");
    }

    private void ShowPluginsInfo()
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

        var pipelines = PipelinePluginRegistry.All
            .Select(p => $"{p.DisplayName} [{p.PipelineId}]")
            .ToList();
        string pipelineLines = pipelines.Count > 0
            ? string.Join(Environment.NewLine, pipelines)
            : "(плагины архивации не загружены)";

        IArchivePipelinePlugin? active = PipelineService.GetActivePlugin();
        string activeLine = active != null
            ? $"{active.DisplayName} (включено: {PipelineService.Settings.PipelineEnabled})"
            : "архивация отключена";

        MessageBox.Show(
            $"Lab 6 — паттерны: Adapter, Facade, Strategy, Observer{Environment.NewLine}{Environment.NewLine}" +
            $"Папка плагинов:{Environment.NewLine}{PluginLoader.PluginsDirectory}{Environment.NewLine}{Environment.NewLine}" +
            $"Загруженные DLL:{Environment.NewLine}{loadedDlls}{Environment.NewLine}{Environment.NewLine}" +
            $"Плагины архивации:{Environment.NewLine}{pipelineLines}{Environment.NewLine}{Environment.NewLine}" +
            $"Активная обработка:{Environment.NewLine}{activeLine}{Environment.NewLine}{Environment.NewLine}" +
            $"Доп. типы в «Добавить»:{Environment.NewLine}{pluginTypeLines}{Environment.NewLine}{Environment.NewLine}" +
            "Адаптер: выберите «Adapter: LegacyPack by Classmate» в настройках.",
            "Плагины", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
}
