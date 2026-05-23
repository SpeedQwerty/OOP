using System.IO;
using System.Reflection;
using System.Windows;
using MediaCatalog.Abstractions.Plugins;
using MediaCatalog.Plugin.Sdk;

namespace MediaCatalog.Plugins;

/// <summary>
/// Discovers plugin DLLs in the Plugins folder, verifies signatures, and initializes IMediaPlugin types.
/// </summary>
public static class PluginLoader
{
    /// <summary>File names of plugin assemblies loaded in the current process.</summary>
    public static IReadOnlyList<string> LoadedFileNames { get; private set; } = Array.Empty<string>();

    /// <summary>Full path to the Plugins directory next to the host executable.</summary>
    public static string PluginsDirectory =>
        Path.Combine(AppContext.BaseDirectory, "Plugins");

    /// <summary>Loads all signed plugins from the default Plugins directory.</summary>
    public static IReadOnlyList<string> LoadFromFolder(ResourceDictionary appResources, bool requireSignature = true)
    {
        string pluginsDir = PluginsDirectory;
        if (!Directory.Exists(pluginsDir))
            Directory.CreateDirectory(pluginsDir);

        var loaded = new List<string>(LoadedFileNames);
        foreach (string dllPath in Directory.EnumerateFiles(pluginsDir, "*.dll"))
        {
            string name = Path.GetFileName(dllPath);
            if (name.Equals("MediaCatalog.dll", StringComparison.OrdinalIgnoreCase)
                || name.Equals("MediaCatalog.Abstractions.dll", StringComparison.OrdinalIgnoreCase)
                || name.Equals("MediaCatalog.Plugin.Sdk.dll", StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                LoadSingle(dllPath, appResources, requireSignature);
                loaded.Add(Path.GetFileName(dllPath));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Plugin load failed: {Path.GetFileName(dllPath)}\n{ex.Message}",
                    "Plugin error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        LoadedFileNames = loaded;
        return loaded;
    }

    /// <summary>Loads one plugin DLL (e.g. from command-line argument).</summary>
    public static void LoadSingle(string dllPath, ResourceDictionary appResources, bool requireSignature = true)
    {
        string fullPath = Path.GetFullPath(dllPath);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Plugin assembly not found.", fullPath);

        if (requireSignature)
            VerifySignature(fullPath);

        var loadContext = new PluginAssemblyLoadContext(fullPath);
        Assembly assembly = loadContext.LoadFromAssemblyPath(fullPath);

        var pluginTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IMediaPlugin).IsAssignableFrom(t))
            .ToList();

        if (pluginTypes.Count == 0)
            throw new InvalidOperationException($"No IMediaPlugin implementation in {Path.GetFileName(fullPath)}.");

        var context = new HostPluginContext(appResources);
        foreach (Type type in pluginTypes)
        {
            if (Activator.CreateInstance(type) is not IMediaPlugin plugin)
                continue;

            plugin.Initialize(context);
        }

        string fileName = Path.GetFileName(fullPath);
        if (!LoadedFileNames.Contains(fileName, StringComparer.OrdinalIgnoreCase))
            LoadedFileNames = LoadedFileNames.Concat(new[] { fileName }).ToArray();
    }

    /// <summary>Verifies .plugin.sig file: integrity hash, validity period, RSA signature.</summary>
    private static void VerifySignature(string dllPath)
    {
        PluginSignaturePayload? payload = PluginSignatureCodec.TryRead(dllPath);
        if (payload == null)
            throw new InvalidOperationException(
                $"Missing signature file. Run PluginSigner for: {Path.GetFileName(dllPath)}");

        string publicKey = PluginTrustKeys.GetPublicKeyXml();
        PluginSignatureVerifier.VerifyOrThrow(dllPath, payload, publicKey);
    }
}
