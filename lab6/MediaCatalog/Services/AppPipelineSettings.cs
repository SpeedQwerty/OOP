using System.IO;
using System.Text.Json;

namespace MediaCatalog.Services;

/// <summary>
/// User preferences for Lab 5 archive pipeline (variant 2) and per-plugin parameters.
/// </summary>
public sealed class AppPipelineSettings
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MediaCatalogLab5",
        "pipeline-settings.json");

    public bool PipelineEnabled { get; set; } = true;

    /// <summary>Selected <see cref="MediaCatalog.Plugin.Sdk.IArchivePipelinePlugin.PipelineId"/>.</summary>
    public string? ActivePipelineId { get; set; } = "archive.gzip";

    /// <summary>Per-plugin setting key/value pairs (compression level, mode, etc.).</summary>
    public Dictionary<string, Dictionary<string, string>> PluginSettings { get; set; } = new();

    public static AppPipelineSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath))
                return new AppPipelineSettings();

            string json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<AppPipelineSettings>(json) ?? new AppPipelineSettings();
        }
        catch
        {
            return new AppPipelineSettings();
        }
    }

    public void Save()
    {
        string? dir = Path.GetDirectoryName(SettingsPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, options));
    }

    public IReadOnlyDictionary<string, string> GetValuesFor(string pipelineId)
    {
        if (PluginSettings.TryGetValue(pipelineId, out var map))
            return map;
        return new Dictionary<string, string>();
    }

    public void SetValuesFor(string pipelineId, IReadOnlyDictionary<string, string> values) =>
        PluginSettings[pipelineId] = new Dictionary<string, string>(values);
}
