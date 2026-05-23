using System.Reflection;
using System.Runtime.Loader;

namespace MediaCatalog.Plugins;

/// <summary>
/// Load context for plugins; shared contract assemblies are taken from the host default context.
/// </summary>
internal sealed class PluginAssemblyLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginAssemblyLoadContext(string pluginPath) : base(isCollectible: false)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        AssemblyName? sharedName = ResolveFromDefaultContext(assemblyName.Name);
        if (sharedName != null)
            return Assembly.Load(sharedName);

        string? path = _resolver.ResolveAssemblyToPath(assemblyName);
        return path != null ? LoadFromAssemblyPath(path) : null;
    }

    /// <summary>Returns an already-loaded assembly from the host so MediaItem is a single type identity.</summary>
    private static AssemblyName? ResolveFromDefaultContext(string? simpleName)
    {
        if (string.IsNullOrEmpty(simpleName))
            return null;

        foreach (Assembly asm in AssemblyLoadContext.Default.Assemblies)
        {
            if (string.Equals(asm.GetName().Name, simpleName, StringComparison.OrdinalIgnoreCase))
                return asm.GetName();
        }

        return null;
    }
}
