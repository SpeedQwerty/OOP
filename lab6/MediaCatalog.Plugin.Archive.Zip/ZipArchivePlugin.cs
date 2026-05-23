using System.IO;
using System.IO.Compression;
using MediaCatalog.Plugin.Sdk;

namespace MediaCatalog.Plugin.Archive.Zip;

/// <summary>
/// Archive pipeline plugin: stores catalog bytes inside a single-entry ZIP container.
/// </summary>
public sealed class ZipArchivePlugin : ArchivePipelinePluginBase, IMediaPlugin
{
    private const string EntryName = "catalog.bin";

    public override string PipelineId => "archive.zip";
    public override string DisplayName => "ZIP Archive";
    public override string Description => "Compresses catalog data with System.IO.Compression.ZipArchive.";

    public string Name => DisplayName;
    string IMediaPlugin.Description => Description;

    public void Initialize(IPluginContext context)
    {
        // Catalog-type plugin not required; pipeline-only registration happens in the host.
    }

    public override IReadOnlyList<PipelineSettingDescriptor> GetSettings() =>
    [
        new PipelineSettingDescriptor
        {
            Key = "level",
            Label = "Compression level (0=store, 9=max)",
            Kind = PipelineSettingKind.Integer,
            DefaultValue = "6"
        }
    ];

    protected override byte[] Compress(byte[] catalogPayload)
    {
        int level = GetIntSetting("level", 6, 0, 9);
        CompressionLevel levelEnum = level switch
        {
            0 => CompressionLevel.NoCompression,
            <= 3 => CompressionLevel.Fastest,
            <= 7 => CompressionLevel.Optimal,
            _ => CompressionLevel.SmallestSize
        };
        using var output = new MemoryStream();
        using (var archive = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true))
        {
            ZipArchiveEntry entry = archive.CreateEntry(EntryName, levelEnum);
            using Stream entryStream = entry.Open();
            entryStream.Write(catalogPayload, 0, catalogPayload.Length);
        }

        return output.ToArray();
    }

    protected override byte[] Decompress(byte[] filePayload)
    {
        using var input = new MemoryStream(filePayload);
        using var archive = new ZipArchive(input, ZipArchiveMode.Read);
        ZipArchiveEntry? entry = archive.GetEntry(EntryName)
            ?? archive.Entries.FirstOrDefault()
            ?? throw new InvalidDataException("ZIP archive does not contain catalog data.");

        using Stream entryStream = entry.Open();
        using var buffer = new MemoryStream();
        entryStream.CopyTo(buffer);
        return buffer.ToArray();
    }
}
