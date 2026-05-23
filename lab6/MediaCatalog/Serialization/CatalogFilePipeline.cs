using System.IO;
using System.Text;
using MediaCatalog.Plugin.Sdk;

namespace MediaCatalog.Serialization;

/// <summary>
/// Wraps compressed catalog payloads with a header so the correct archive plugin can decompress on load.
/// </summary>
public static class CatalogFilePipeline
{
    private static readonly byte[] Magic = Encoding.ASCII.GetBytes("MC5A");

    /// <summary>
    /// Writes catalog bytes, optionally compressed by <paramref name="plugin"/>.
    /// </summary>
    public static void WriteFile(string filePath, byte[] catalogBytes, IArchivePipelinePlugin? plugin)
    {
        byte[] payload = plugin != null ? plugin.ProcessBeforeSave(catalogBytes) : catalogBytes;

        using var fs = File.Create(filePath);
        if (plugin == null)
        {
            fs.Write(payload, 0, payload.Length);
            return;
        }

        byte[] idBytes = Encoding.UTF8.GetBytes(plugin.PipelineId);
        if (idBytes.Length > 255)
            throw new InvalidOperationException("Pipeline id is too long for the file header.");

        fs.Write(Magic, 0, Magic.Length);
        fs.WriteByte((byte)idBytes.Length);
        fs.Write(idBytes, 0, idBytes.Length);
        fs.Write(payload, 0, payload.Length);
    }

    /// <summary>
    /// Reads a file and returns raw catalog bytes (decompressing when the Lab 5 header is present).
    /// </summary>
    public static byte[] ReadFile(string filePath, Func<string, IArchivePipelinePlugin?> resolvePlugin)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        if (fileBytes.Length < Magic.Length + 2)
            return fileBytes;

        if (!fileBytes.AsSpan(0, Magic.Length).SequenceEqual(Magic))
            return fileBytes;

        int idLength = fileBytes[Magic.Length];
        int payloadOffset = Magic.Length + 1 + idLength;
        if (fileBytes.Length < payloadOffset)
            throw new InvalidDataException("Corrupted archive catalog file (truncated header).");

        string pipelineId = Encoding.UTF8.GetString(fileBytes, Magic.Length + 1, idLength);
        byte[] compressed = fileBytes.AsSpan(payloadOffset).ToArray();

        IArchivePipelinePlugin? plugin = resolvePlugin(pipelineId)
            ?? throw new InvalidOperationException(
                $"File was saved with archive plugin '{pipelineId}', but that plugin is not loaded.");

        return plugin.ProcessAfterLoad(compressed);
    }
}
