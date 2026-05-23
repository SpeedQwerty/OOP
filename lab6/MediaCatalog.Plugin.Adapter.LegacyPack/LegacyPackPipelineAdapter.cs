using FriendPlugin.LegacyPack;
using MediaCatalog.Plugin.Sdk;

namespace MediaCatalog.Plugin.Adapter.LegacyPack;

/// <summary>
/// Adapter pattern (GoF): translates the classmate <see cref="ILegacyPackService"/> API
/// into the host contract <see cref="IArchivePipelinePlugin"/> so the foreign DLL can be
/// selected in settings and used on save/load without modifying the host.
/// </summary>
public sealed class LegacyPackPipelineAdapter : IArchivePipelinePlugin
{
    private readonly ILegacyPackService _legacyService;
    private LegacyPackOptions _options = new() { Strength = 5, AddVendorHeader = true };

    public LegacyPackPipelineAdapter(ILegacyPackService legacyService)
    {
        _legacyService = legacyService;
    }

    public string PipelineId => "adapter.legacy-pack";

    public string DisplayName => $"Adapter: {_legacyService.VendorName}";

    public string Description =>
        "Adapter plugin wrapping a classmate LegacyPack DLL (Lab 6).";

    public IReadOnlyList<PipelineSettingDescriptor> GetSettings() =>
    [
        new PipelineSettingDescriptor
        {
            Key = "strength",
            Label = "Legacy strength (1-9)",
            Kind = PipelineSettingKind.Integer,
            DefaultValue = "5"
        },
        new PipelineSettingDescriptor
        {
            Key = "vendorHeader",
            Label = "Add classmate vendor header",
            Kind = PipelineSettingKind.Boolean,
            DefaultValue = "true"
        }
    ];

    public void ApplySettings(IReadOnlyDictionary<string, string> values)
    {
        int strength = int.TryParse(Get(values, "strength", "5"), out int s) ? Math.Clamp(s, 1, 9) : 5;
        bool header = bool.TryParse(Get(values, "vendorHeader", "true"), out bool h) && h;
        _options = new LegacyPackOptions { Strength = strength, AddVendorHeader = header };
    }

    public byte[] ProcessBeforeSave(byte[] catalogPayload) =>
        _legacyService.PackData(catalogPayload, _options);

    public byte[] ProcessAfterLoad(byte[] filePayload) =>
        _legacyService.UnpackData(filePayload);

    private static string Get(IReadOnlyDictionary<string, string> values, string key, string fallback) =>
        values.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v) ? v : fallback;
}
