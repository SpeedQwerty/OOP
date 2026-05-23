using MediaCatalog.Plugin.Sdk;
using MediaCatalog.Plugins;

namespace MediaCatalog.Services;

/// <summary>
/// Applies stored settings and resolves the active archive pipeline plugin for save/load.
/// </summary>
public static class PipelineService
{
    public static AppPipelineSettings Settings { get; private set; } = AppPipelineSettings.Load();

    public static void ReloadSettings() => Settings = AppPipelineSettings.Load();

    public static void SaveSettings() => Settings.Save();

    /// <summary>Applies persisted settings to every registered pipeline plugin.</summary>
    public static void ApplySettingsToAllPlugins()
    {
        foreach (var plugin in PipelinePluginRegistry.All)
        {
            var values = Settings.GetValuesFor(plugin.PipelineId);
            if (values.Count == 0)
            {
                var defaults = plugin.GetSettings()
                    .ToDictionary(s => s.Key, s => s.DefaultValue, StringComparer.OrdinalIgnoreCase);
                plugin.ApplySettings(defaults);
            }
            else
            {
                plugin.ApplySettings(values);
            }
        }
    }

    /// <summary>Returns the active archive plugin when pipeline processing is enabled.</summary>
    public static IArchivePipelinePlugin? GetActivePlugin()
    {
        if (!Settings.PipelineEnabled || string.IsNullOrWhiteSpace(Settings.ActivePipelineId))
            return null;

        return PipelinePluginRegistry.TryGet(Settings.ActivePipelineId);
    }
}
