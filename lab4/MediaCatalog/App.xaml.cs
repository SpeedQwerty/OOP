using System.IO;
using System.Linq;
using System.Windows;
using MediaCatalog.Models;
using MediaCatalog.Plugins;
using MediaCatalog.Serialization;

namespace MediaCatalog;

public partial class App : Application
{
    /// <summary>Optional plugin path from command line: MediaCatalog.exe --plugin path\to\plugin.dll</summary>
    private const string PluginArgPrefix = "--plugin";

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        TypeBootstrap.EnsureLoaded();

        PluginCatalog.RegisterBuiltIn(new MediaCatalog.Abstractions.Models.MediaItem[]
        {
            new Book(), new Magazine(), new EBook(), new Movie(), new MusicAlbum(), new Audiobook()
        });

        bool requireSignature = !e.Args.Contains("--no-plugin-signature");
        var loaded = PluginLoader.LoadFromFolder(Resources, requireSignature);

        string? extraPlugin = GetPluginArg(e.Args);
        if (!string.IsNullOrEmpty(extraPlugin))
        {
            try
            {
                PluginLoader.LoadSingle(extraPlugin, Resources, requireSignature);
                loaded = loaded.Concat(new[] { Path.GetFileName(extraPlugin) }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plugin load error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        if (loaded.Count > 0)
            Current.MainWindow?.Title += $" | Plugins: {string.Join(", ", loaded)}";
    }

    private static string? GetPluginArg(string[] args)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i].Equals(PluginArgPrefix, StringComparison.OrdinalIgnoreCase))
                return args[i + 1];
        }
        return null;
    }
}
