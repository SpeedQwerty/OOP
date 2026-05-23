namespace MediaCatalog.Plugin.Sdk;

/// <summary>
/// Contract implemented by every dynamically loaded plugin assembly.
/// The host discovers implementations via reflection without recompilation.
/// </summary>
public interface IMediaPlugin
{
    /// <summary>Display name of the plugin module.</summary>
    string Name { get; }

    /// <summary>Short description shown in the status bar after load.</summary>
    string Description { get; }

    /// <summary>
    /// Registers new media types, serializers, and optional WPF UI resources.
    /// </summary>
    void Initialize(IPluginContext context);
}
