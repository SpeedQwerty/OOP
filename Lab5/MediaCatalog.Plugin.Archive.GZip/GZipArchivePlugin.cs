using System.IO;
using System.IO.Compression;
using MediaCatalog.Plugin.Sdk;

namespace MediaCatalog.Plugin.Archive.GZip;

/// <summary>
/// Archive pipeline plugin: GZip compression for serialized catalog bytes.
/// </summary>
public sealed class GZipArchivePlugin : ArchivePipelinePluginBase, IMediaPlugin
{
    public override string PipelineId => "archive.gzip";
    public override string DisplayName => "GZip Archive";
    public override string Description => "Compresses catalog data with GZipStream.";

    public string Name => DisplayName;
    string IMediaPlugin.Description => Description;

    public void Initialize(IPluginContext context) { }

    public override IReadOnlyList<PipelineSettingDescriptor> GetSettings() =>
    [
        new PipelineSettingDescriptor
        {
            Key = "mode",
            Label = "Compression mode",
            Kind = PipelineSettingKind.Choice,
            DefaultValue = "Optimal",
            Choices = ["Fastest", "Optimal", "SmallestSize"]
        }
    ];

    protected override byte[] Compress(byte[] catalogPayload)
    {
        CompressionLevel level = ParseCompressionLevel(GetSetting("mode", "Optimal"));
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, level, leaveOpen: true))
            gzip.Write(catalogPayload, 0, catalogPayload.Length);

        return output.ToArray();
    }

    protected override byte[] Decompress(byte[] filePayload)
    {
        using var input = new MemoryStream(filePayload);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }

    private static CompressionLevel ParseCompressionLevel(string value) =>
        Enum.TryParse<CompressionLevel>(value, ignoreCase: true, out var level)
            ? level
            : CompressionLevel.Optimal;
}
