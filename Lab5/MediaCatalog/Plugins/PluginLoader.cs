using System.IO;
using System.Reflection;
using System.Windows;
using MediaCatalog.Abstractions.Plugins;
using MediaCatalog.Plugin.Sdk;
using MediaCatalog.Services;

namespace MediaCatalog.Plugins;

/// <summary>
/// Discovers plugin DLLs in the Plugins folder, verifies signatures, and initializes IMediaPlugin types.
/// Also registers <see cref="IArchivePipelinePlugin"/> implementations for Lab 5 archiving.
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

        var loaded = new List<string>();
        foreach (string dllPath in Directory.EnumerateFiles(pluginsDir, "*.dll"))
        {
            string name = Path.GetFileName(dllPath);
            if (IsHostDependency(name))
                continue;

            try
            {
                LoadSingle(dllPath, appResources, requireSignature);
                loaded.Add(name);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Plugin load failed: {name}\n{ex.Message}",
                    "Plugin error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        LoadedFileNames = loaded;
        PipelineService.ApplySettingsToAllPlugins();
        return loaded;
    }

    /// <summary>Loads one plugin DLL from a user-selected path or command line.</summary>
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
            .Where(t => t.IsClass && !t.IsAbstract &&
                        (typeof(IMediaPlugin).IsAssignableFrom(t) ||
                         typeof(IArchivePipelinePlugin).IsAssignableFrom(t)))
            .Distinct()
            .ToList();

        if (pluginTypes.Count == 0)
            throw new InvalidOperationException($"No plugin types in {Path.GetFileName(fullPath)}.");

        var context = new HostPluginContext(appResources);
        foreach (Type type in pluginTypes)
        {
            object? instance = Activator.CreateInstance(type);
            if (instance == null)
                continue;

            if (instance is IMediaPlugin mediaPlugin)
                mediaPlugin.Initialize(context);

            if (instance is IArchivePipelinePlugin pipelinePlugin)
                PipelinePluginRegistry.Register(pipelinePlugin);
        }

        string fileName = Path.GetFileName(fullPath);
        if (!LoadedFileNames.Contains(fileName, StringComparer.OrdinalIgnoreCase))
            LoadedFileNames = LoadedFileNames.Concat(new[] { fileName }).ToArray();

        PipelineService.ApplySettingsToAllPlugins();
    }

    private static bool IsHostDependency(string fileName) =>
        fileName.Equals("MediaCatalog.dll", StringComparison.OrdinalIgnoreCase)
        || fileName.Equals("MediaCatalog.Abstractions.dll", StringComparison.OrdinalIgnoreCase)
        || fileName.Equals("MediaCatalog.Plugin.Sdk.dll", StringComparison.OrdinalIgnoreCase);

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
