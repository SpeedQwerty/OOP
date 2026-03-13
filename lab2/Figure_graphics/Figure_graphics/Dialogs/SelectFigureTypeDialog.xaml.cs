using System.Windows;

namespace Figure_graphics.Dialogs
{
    /// <summary>
    /// Dialog for selecting figure type when using mouse input mode.
    /// </summary>
    public partial class SelectFigureTypeDialog : Window
    {
        public string? SelectedType { get; private set; }

        public SelectFigureTypeDialog()
        {
            InitializeComponent();
            foreach (var name in Factory.FigureFactory.GetRegisteredTypes())
            {
                TypeList.Items.Add(name);
            }
            if (TypeList.Items.Count > 0)
                TypeList.SelectedIndex = 0;
        }

        private void TypeList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedType = TypeList.SelectedItem?.ToString();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedType = TypeList.SelectedItem?.ToString();
            DialogResult = !string.IsNullOrEmpty(SelectedType);
            Close();
        }
    }
}
