using System.Windows;
using MediaCatalog.Abstractions.Models;

namespace MediaCatalog.Plugin.Sdk;

/// <summary>
/// Host-provided API that plugins use to extend the catalog and UI.
/// </summary>
public interface IPluginContext
{
    /// <summary>Registers a prototype instance for the "Add item" dialog.</summary>
    void RegisterCatalogType(MediaItem prototype);

    /// <summary>Merges WPF resources (e.g. DataTemplates) into the application.</summary>
    void MergeResources(ResourceDictionary resources);
}
