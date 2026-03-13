using System.Windows;

namespace MediaCatalog.Views;

public partial class AddItemDialog : Window
{
    public MediaCatalog.Models.MediaItem? SelectedType { get; private set; }

    public AddItemDialog(MediaCatalog.Models.MediaItem[] types)
    {
        InitializeComponent();
        TypeList.ItemsSource = types;
    }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        SelectedType = TypeList.SelectedItem as MediaCatalog.Models.MediaItem;
        DialogResult = true;
        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
