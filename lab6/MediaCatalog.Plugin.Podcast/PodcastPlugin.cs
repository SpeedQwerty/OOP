using System.Windows;
using MediaCatalog.Plugin.Podcast.Models;
using MediaCatalog.Plugin.Sdk;

namespace MediaCatalog.Plugin.Podcast;

/// <summary>
/// Entry point for the podcast plugin; discovered by the host via IMediaPlugin.
/// </summary>
public sealed class PodcastPlugin : IMediaPlugin
{
    public string Name => "Podcast Extension";
    public string Description => "Adds Podcast media type to the catalog";

    public void Initialize(IPluginContext context)
    {
        _ = new Models.Podcast();

        context.RegisterCatalogType(new Models.Podcast());

        var resources = new ResourceDictionary
        {
            Source = new Uri("/MediaCatalog.Plugin.Podcast;component/PodcastResources.xaml", UriKind.Relative)
        };
        context.MergeResources(resources);
    }
}
