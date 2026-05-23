using FriendPlugin.LegacyPack;
using MediaCatalog.Plugin.Sdk;

namespace MediaCatalog.Plugin.Adapter.LegacyPack;

/// <summary>
/// Host entry point: exposes the Adapter-wrapped classmate pipeline via both plugin contracts.
/// </summary>
public sealed class LegacyPackAdapterPlugin : IMediaPlugin, IArchivePipelinePlugin
{
    private readonly LegacyPackPipelineAdapter _adapter = new(new LegacyPackService());

    public string Name => "LegacyPack Adapter (classmate)";
    public string Description => "Adapter pattern bridge for FriendPlugin.LegacyPack";

    public void Initialize(IPluginContext context)
    {
        // Media types are not extended; only the archive pipeline is adapted.
    }

    // Delegate IArchivePipelinePlugin to the adapter instance.
    public string PipelineId => _adapter.PipelineId;
    public string DisplayName => _adapter.DisplayName;
    string IArchivePipelinePlugin.Description => _adapter.Description;
    public IReadOnlyList<PipelineSettingDescriptor> GetSettings() => _adapter.GetSettings();
    public void ApplySettings(IReadOnlyDictionary<string, string> values) => _adapter.ApplySettings(values);
    public byte[] ProcessBeforeSave(byte[] catalogPayload) => _adapter.ProcessBeforeSave(catalogPayload);
    public byte[] ProcessAfterLoad(byte[] filePayload) => _adapter.ProcessAfterLoad(filePayload);
}
