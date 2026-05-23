using System.Windows;
using System.Windows.Controls;
using MediaCatalog.Plugin.Sdk;
using MediaCatalog.Plugins;
using MediaCatalog.Services;
using Microsoft.Win32;

namespace MediaCatalog.Views;

/// <summary>
/// Settings dialog: archive pipeline selection, per-plugin parameters, and plugin loading UI.
/// </summary>
public partial class SettingsWindow : Window
{
    private readonly Dictionary<string, FrameworkElement> _settingControls = new(StringComparer.OrdinalIgnoreCase);

    public SettingsWindow()
    {
        InitializeComponent();
        PluginsFolderText.Text = $"Папка: {PluginLoader.PluginsDirectory}";
        LoadedPluginsList.ItemsSource = PluginLoader.LoadedFileNames;
        PipelineEnabledCheck.IsChecked = PipelineService.Settings.PipelineEnabled;
        BindPipelineCombo();
    }

    private void BindPipelineCombo()
    {
        var plugins = PipelinePluginRegistry.All.ToList();
        PipelineCombo.ItemsSource = plugins;

        if (plugins.Count == 0)
        {
            PipelineCombo.IsEnabled = false;
            return;
        }

        string? activeId = PipelineService.Settings.ActivePipelineId;
        PipelineCombo.SelectedItem = plugins.FirstOrDefault(p => p.PipelineId == activeId) ?? plugins[0];
    }

    private void PipelineCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PipelineCombo.SelectedItem is not IArchivePipelinePlugin plugin)
            return;

        BuildSettingsPanel(plugin);
    }

    /// <summary>Builds dynamic controls from plugin <see cref="IArchivePipelinePlugin.GetSettings"/>.</summary>
    private void BuildSettingsPanel(IArchivePipelinePlugin plugin)
    {
        SettingsPanel.Children.Clear();
        _settingControls.Clear();

        var saved = PipelineService.Settings.GetValuesFor(plugin.PipelineId);
        foreach (var descriptor in plugin.GetSettings())
        {
            var row = new StackPanel { Margin = new Thickness(0, 0, 0, 8) };
            row.Children.Add(new TextBlock { Text = descriptor.Label, FontWeight = FontWeights.SemiBold });

            string initial = saved.TryGetValue(descriptor.Key, out var v) ? v : descriptor.DefaultValue;
            FrameworkElement editor = descriptor.Kind switch
            {
                PipelineSettingKind.Choice => CreateChoiceEditor(descriptor, initial),
                PipelineSettingKind.Boolean => CreateBooleanEditor(initial),
                _ => CreateIntegerEditor(initial)
            };

            _settingControls[descriptor.Key] = editor;
            row.Children.Add(editor);
            SettingsPanel.Children.Add(row);
        }

        if (plugin.GetSettings().Count == 0)
            SettingsPanel.Children.Add(new TextBlock
            {
                Text = "У этого плагина нет дополнительных параметров.",
                Foreground = System.Windows.Media.Brushes.Gray
            });
    }

    private static ComboBox CreateChoiceEditor(PipelineSettingDescriptor descriptor, string initial)
    {
        var combo = new ComboBox { Margin = new Thickness(0, 4, 0, 0) };
        if (descriptor.Choices != null)
            foreach (string choice in descriptor.Choices)
                combo.Items.Add(choice);
        combo.SelectedItem = descriptor.Choices?.Contains(initial) == true ? initial : descriptor.DefaultValue;
        return combo;
    }

    private static CheckBox CreateBooleanEditor(string initial) =>
        new() { IsChecked = bool.TryParse(initial, out var b) && b, Margin = new Thickness(0, 4, 0, 0) };

    private static TextBox CreateIntegerEditor(string initial) =>
        new() { Text = initial, Margin = new Thickness(0, 4, 0, 0) };

    private Dictionary<string, string> CollectSettingsFromUi()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in _settingControls)
        {
            result[pair.Key] = pair.Value switch
            {
                ComboBox combo => combo.SelectedItem?.ToString() ?? "",
                CheckBox check => (check.IsChecked == true).ToString(),
                TextBox text => text.Text,
                _ => ""
            };
        }

        return result;
    }

    private void SaveClick(object sender, RoutedEventArgs e)
    {
        if (PipelineCombo.SelectedItem is IArchivePipelinePlugin plugin)
        {
            var values = CollectSettingsFromUi();
            plugin.ApplySettings(values);
            PipelineService.Settings.SetValuesFor(plugin.PipelineId, values);
            PipelineService.Settings.ActivePipelineId = plugin.PipelineId;
        }

        PipelineService.Settings.PipelineEnabled = PipelineEnabledCheck.IsChecked == true;
        PipelineService.SaveSettings();
        DialogResult = true;
        Close();
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void LoadPluginClick(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = "Plugin assemblies (*.dll)|*.dll",
            Title = "Выберите файл плагина"
        };
        if (dlg.ShowDialog() != true)
            return;

        try
        {
            PluginLoader.LoadSingle(dlg.FileName, Application.Current.Resources);
            LoadedPluginsList.ItemsSource = null;
            LoadedPluginsList.ItemsSource = PluginLoader.LoadedFileNames;
            BindPipelineCombo();
            MessageBox.Show("Плагин загружен. Настройки архивации обновлены.", "Плагины",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка загрузки плагина", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ReloadFolderClick(object sender, RoutedEventArgs e)
    {
        try
        {
            PluginLoader.LoadFromFolder(Application.Current.Resources);
            LoadedPluginsList.ItemsSource = null;
            LoadedPluginsList.ItemsSource = PluginLoader.LoadedFileNames;
            BindPipelineCombo();
            MessageBox.Show("Плагины из папки Plugins перезагружены.", "Плагины",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
