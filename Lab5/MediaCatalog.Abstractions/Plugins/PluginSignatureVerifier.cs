using System.Security.Cryptography;

namespace MediaCatalog.Abstractions.Plugins;

/// <summary>
/// Verifies plugin assembly integrity and RSA signature against the host trust key.
/// </summary>
public static class PluginSignatureVerifier
{
    /// <summary>
    /// Validates hash, activation window, and digital signature of a plugin DLL.
    /// </summary>
    public static void VerifyOrThrow(string pluginDllPath, PluginSignaturePayload payload, string publicKeyXml)
    {
        byte[] dllBytes = File.ReadAllBytes(pluginDllPath);
        byte[] actualHash = SHA256.HashData(dllBytes);

        if (!actualHash.AsSpan().SequenceEqual(payload.AssemblyHash))
            throw new InvalidOperationException("Plugin assembly hash does not match the signature file.");

        DateTime now = DateTime.UtcNow;
        if (now < payload.NotBeforeUtc)
            throw new InvalidOperationException($"Plugin is not active yet (valid from {payload.NotBeforeUtc:u}).");
        if (now > payload.NotAfterUtc)
            throw new InvalidOperationException($"Plugin has expired (valid until {payload.NotAfterUtc:u}).");

        byte[] signedContent = PluginSignatureCodec.BuildSignedContent(
            payload.AssemblyHash, payload.NotBeforeUtc, payload.NotAfterUtc);

        using var rsa = RSA.Create();
        rsa.FromXmlString(publicKeyXml);
        bool ok = rsa.VerifyData(signedContent, payload.Signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
        if (!ok)
            throw new InvalidOperationException("Plugin digital signature is invalid.");
    }
}
