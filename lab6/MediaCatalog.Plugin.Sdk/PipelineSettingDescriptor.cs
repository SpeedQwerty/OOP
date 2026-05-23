namespace MediaCatalog.Plugin.Sdk;

/// <summary>
/// Describes one configurable option exposed by an archive pipeline plugin.
/// </summary>
public sealed class PipelineSettingDescriptor
{
    public required string Key { get; init; }
    public required string Label { get; init; }
    public PipelineSettingKind Kind { get; init; } = PipelineSettingKind.Integer;
    public required string DefaultValue { get; init; }
    public string[]? Choices { get; init; }
}
