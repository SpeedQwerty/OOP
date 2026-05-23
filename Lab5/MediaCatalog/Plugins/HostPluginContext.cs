using System.Windows;
using MediaCatalog.Abstractions.Models;
using MediaCatalog.Plugin.Sdk;

namespace MediaCatalog.Plugins;

/// <summary>
/// Host implementation of <see cref="IPluginContext"/> passed to each loaded plugin.
/// </summary>
public sealed class HostPluginContext : IPluginContext
{
    private readonly ResourceDictionary _applicationResources;

    public HostPluginContext(ResourceDictionary applicationResources)
    {
        _applicationResources = applicationResources;
    }

    public void RegisterCatalogType(MediaItem prototype)
    {
        PluginCatalog.RegisterFromPlugin(prototype);
    }

    public void MergeResources(ResourceDictionary resources)
    {
        _applicationResources.MergedDictionaries.Add(resources);
    }
}
