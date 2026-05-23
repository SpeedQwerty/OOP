using System.IO;
using System.IO.Compression;
using MediaCatalog.Plugin.Sdk;

namespace MediaCatalog.Plugin.Archive.Brotli;

/// <summary>
/// Archive pipeline plugin: Brotli compression for serialized catalog bytes.
/// </summary>
public sealed class BrotliArchivePlugin : ArchivePipelinePluginBase, IMediaPlugin
{
    public override string PipelineId => "archive.brotli";
    public override string DisplayName => "Brotli Archive";
    public override string Description => "Compresses catalog data with BrotliStream.";

    public string Name => DisplayName;
    string IMediaPlugin.Description => Description;

    public void Initialize(IPluginContext context) { }

    public override IReadOnlyList<PipelineSettingDescriptor> GetSettings() =>
    [
        new PipelineSettingDescriptor
        {
            Key = "quality",
            Label = "Quality (1=fast, 11=max compression)",
            Kind = PipelineSettingKind.Integer,
            DefaultValue = "5"
        }
    ];

    protected override byte[] Compress(byte[] catalogPayload)
    {
        int quality = GetIntSetting("quality", 5, 1, 11);
        using var output = new MemoryStream();
        var options = new BrotliCompressionOptions { Quality = quality };
        using (var brotli = new BrotliStream(output, options, leaveOpen: true))
            brotli.Write(catalogPayload, 0, catalogPayload.Length);

        return output.ToArray();
    }

    protected override byte[] Decompress(byte[] filePayload)
    {
        using var input = new MemoryStream(filePayload);
        using var brotli = new BrotliStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        brotli.CopyTo(output);
        return output.ToArray();
    }
}
