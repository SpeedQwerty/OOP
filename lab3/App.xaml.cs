using MediaCatalog.Serialization;
using System.Windows;

namespace MediaCatalog;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        TypeBootstrap.EnsureLoaded();
    }
}

