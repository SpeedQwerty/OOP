using System.IO;
using System.Text;

namespace MediaCatalog.Abstractions.Plugins;

/// <summary>
/// Serializes and deserializes plugin signature files to disk.
/// </summary>
public static class PluginSignatureCodec
{
    /// <summary>
    /// Writes a signature file for the given plugin assembly path.
    /// </summary>
    public static void Write(string pluginDllPath, PluginSignaturePayload payload)
    {
        string sigPath = pluginDllPath + PluginSignaturePayload.FileExtension;
        using var fs = File.Create(sigPath);
        using var writer = new BinaryWriter(fs, Encoding.UTF8, leaveOpen: false);

        writer.Write(PluginSignaturePayload.Magic);
        writer.Write(payload.NotBeforeUtc.ToUniversalTime().Ticks);
        writer.Write(payload.NotAfterUtc.ToUniversalTime().Ticks);
        writer.Write(payload.AssemblyHash.Length);
        writer.Write(payload.AssemblyHash);
        writer.Write(payload.Signature.Length);
        writer.Write(payload.Signature);
    }

    /// <summary>
    /// Reads a signature file; returns null if the file is missing.
    /// </summary>
    public static PluginSignaturePayload? TryRead(string pluginDllPath)
    {
        string sigPath = pluginDllPath + PluginSignaturePayload.FileExtension;
        if (!File.Exists(sigPath))
            return null;

        using var fs = File.OpenRead(sigPath);
        using var reader = new BinaryReader(fs, Encoding.UTF8, leaveOpen: false);

        string magic = reader.ReadString();
        if (magic != PluginSignaturePayload.Magic)
            throw new InvalidDataException("Invalid plugin signature magic header.");

        var notBefore = new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
        var notAfter = new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
        int hashLen = reader.ReadInt32();
        byte[] hash = reader.ReadBytes(hashLen);
        int sigLen = reader.ReadInt32();
        byte[] signature = reader.ReadBytes(sigLen);

        return new PluginSignaturePayload
        {
            NotBeforeUtc = notBefore,
            NotAfterUtc = notAfter,
            AssemblyHash = hash,
            Signature = signature
        };
    }

    /// <summary>
    /// Builds canonical bytes that are signed and verified (hash + validity window).
    /// </summary>
    public static byte[] BuildSignedContent(byte[] assemblyHash, DateTime notBeforeUtc, DateTime notAfterUtc)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);
        writer.Write(notBeforeUtc.ToUniversalTime().Ticks);
        writer.Write(notAfterUtc.ToUniversalTime().Ticks);
        writer.Write(assemblyHash.Length);
        writer.Write(assemblyHash);
        return ms.ToArray();
    }
}
