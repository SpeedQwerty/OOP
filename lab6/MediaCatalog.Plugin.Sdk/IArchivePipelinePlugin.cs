namespace MediaCatalog.Plugin.Sdk;

/// <summary>
/// Variant 2 (archiving): transforms serialized catalog bytes before save and after load.
/// Implemented by dedicated archive plugins; discovered together with <see cref="IMediaPlugin"/>.
/// </summary>
public interface IArchivePipelinePlugin
{
    /// <summary>Stable identifier written into the file header (UTF-8).</summary>
    string PipelineId { get; }

    /// <summary>Display name in the settings menu.</summary>
    string DisplayName { get; }

    /// <summary>Short description for the settings UI.</summary>
    string Description { get; }

    /// <summary>Compresses raw catalog bytes immediately before writing to disk.</summary>
    byte[] ProcessBeforeSave(byte[] catalogPayload);

    /// <summary>Decompresses file payload immediately after reading from disk.</summary>
    byte[] ProcessAfterLoad(byte[] filePayload);

    /// <summary>Returns plugin-specific options (compression level, algorithm variant, etc.).</summary>
    IReadOnlyList<PipelineSettingDescriptor> GetSettings();

    /// <summary>Applies values chosen in the settings dialog before the next save/load cycle.</summary>
    void ApplySettings(IReadOnlyDictionary<string, string> values);
}
