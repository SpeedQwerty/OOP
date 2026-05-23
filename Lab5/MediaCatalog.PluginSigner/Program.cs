using System.Security.Cryptography;
using MediaCatalog.Abstractions.Plugins;

namespace MediaCatalog.PluginSigner;

/// <summary>
/// Console tool that signs plugin DLLs (integrity + activation window + RSA-PSS).
/// </summary>
internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: MediaCatalog.PluginSigner <path-to-plugin.dll> [days-valid]");
            return 1;
        }

        string dllPath = Path.GetFullPath(args[0]);
        int daysValid = args.Length > 1 && int.TryParse(args[1], out int d) ? d : 365;

        if (!File.Exists(dllPath))
        {
            Console.Error.WriteLine($"File not found: {dllPath}");
            return 1;
        }

        string privateKeyXml = File.ReadAllText(ResolvePrivateKeyPath());
        byte[] hash = SHA256.HashData(File.ReadAllBytes(dllPath));

        DateTime notBefore = DateTime.UtcNow.AddMinutes(-5);
        DateTime notAfter = notBefore.AddDays(daysValid);
        byte[] signedContent = PluginSignatureCodec.BuildSignedContent(hash, notBefore, notAfter);

        byte[] signature;
        using (var rsa = RSA.Create())
        {
            rsa.FromXmlString(privateKeyXml);
            signature = rsa.SignData(signedContent, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
        }

        PluginSignatureCodec.Write(dllPath, new PluginSignaturePayload
        {
            NotBeforeUtc = notBefore,
            NotAfterUtc = notAfter,
            AssemblyHash = hash,
            Signature = signature
        });

        Console.WriteLine($"Signed: {dllPath}");
        Console.WriteLine($"  Valid: {notBefore:u} — {notAfter:u}");
        return 0;
    }

    private static string ResolvePrivateKeyPath()
    {
        string[] candidates =
        {
            Path.Combine(Directory.GetCurrentDirectory(), "Keys", "plugin-trust.private.xml"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "Keys", "plugin-trust.private.xml"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Keys", "plugin-trust.private.xml"),
        };

        foreach (string path in candidates)
        {
            string full = Path.GetFullPath(path);
            if (File.Exists(full))
                return full;
        }

        throw new FileNotFoundException("Private key not found: Keys/plugin-trust.private.xml");
    }
}
