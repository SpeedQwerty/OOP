using System.IO;
using System.IO.Compression;

namespace FriendPlugin.LegacyPack;

/// <summary>
/// Default implementation of the classmate pack service (Deflate + optional vendor header "LPK1").
/// </summary>
public sealed class LegacyPackService : ILegacyPackService
{
    private static readonly byte[] VendorMagic = "LPK1"u8.ToArray();

    public string VendorName => "LegacyPack by Classmate";

    /// <inheritdoc />
    public byte[] PackData(byte[] plainBytes, LegacyPackOptions options)
    {
        using var body = new MemoryStream();
        CompressionLevel level = MapStrength(options.Strength);
        using (var deflate = new DeflateStream(body, level, leaveOpen: true))
            deflate.Write(plainBytes, 0, plainBytes.Length);

        byte[] payload = body.ToArray();
        if (!options.AddVendorHeader)
            return payload;

        var result = new byte[VendorMagic.Length + payload.Length];
        VendorMagic.CopyTo(result, 0);
        payload.CopyTo(result, VendorMagic.Length);
        return result;
    }

    /// <inheritdoc />
    public byte[] UnpackData(byte[] packedBytes)
    {
        ReadOnlySpan<byte> span = packedBytes;
        if (span.Length >= VendorMagic.Length && span[..VendorMagic.Length].SequenceEqual(VendorMagic))
            span = span[VendorMagic.Length..];

        using var input = new MemoryStream(span.ToArray());
        using var deflate = new DeflateStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        deflate.CopyTo(output);
        return output.ToArray();
    }

    private static CompressionLevel MapStrength(int strength) => strength switch
    {
        <= 3 => CompressionLevel.Fastest,
        <= 6 => CompressionLevel.Optimal,
        _ => CompressionLevel.SmallestSize
    };
}
