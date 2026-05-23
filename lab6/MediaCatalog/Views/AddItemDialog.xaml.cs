using System.Windows;
using MediaCatalog.Abstractions.Models;

namespace MediaCatalog.Views;

public partial class AddItemDialog : Window
{
    public MediaItem? SelectedType { get; private set; }

    public AddItemDialog(MediaItem[] types)
    {
        InitializeComponent();
        TypeList.ItemsSource = types;
    }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        SelectedType = TypeList.SelectedItem as MediaItem;
        DialogResult = true;
        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
