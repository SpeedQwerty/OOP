namespace MediaCatalog.Plugin.Sdk;

/// <summary>
/// Base class for archive pipeline plugins: wires settings and delegates to Compress/Decompress.
/// </summary>
public abstract class ArchivePipelinePluginBase : IArchivePipelinePlugin
{
    private IReadOnlyDictionary<string, string> _settings = new Dictionary<string, string>();

    public abstract string PipelineId { get; }
    public abstract string DisplayName { get; }
    public abstract string Description { get; }

    public virtual IReadOnlyList<PipelineSettingDescriptor> GetSettings() =>
        Array.Empty<PipelineSettingDescriptor>();

    public void ApplySettings(IReadOnlyDictionary<string, string> values) =>
        _settings = new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase);

    public byte[] ProcessBeforeSave(byte[] catalogPayload) => Compress(catalogPayload);

    public byte[] ProcessAfterLoad(byte[] filePayload) => Decompress(filePayload);

    /// <summary>Reads a setting or returns the default from <paramref name="fallback"/>.</summary>
    protected string GetSetting(string key, string fallback) =>
        _settings.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : fallback;

    protected int GetIntSetting(string key, int fallback, int min, int max)
    {
        if (!int.TryParse(GetSetting(key, fallback.ToString()), out int value))
            return fallback;
        return Math.Clamp(value, min, max);
    }

    protected abstract byte[] Compress(byte[] catalogPayload);

    protected abstract byte[] Decompress(byte[] filePayload);
}
