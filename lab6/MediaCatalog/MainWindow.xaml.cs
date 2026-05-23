using System.Windows;

namespace MediaCatalog;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnMainWindowLoaded;
    }

    /// <summary>Applies plugin/archive info to the title after the window is created (OnStartup runs earlier).</summary>
    private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(App.StartupTitleSuffix))
            Title += App.StartupTitleSuffix;
    }
}
