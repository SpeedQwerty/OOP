using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Figure_graphics.Factory;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Dialogs
{
    /// <summary>
    /// Dialog for creating figures by manually entering parameters.
    /// Uses FigureFactory.GetParamOrder - no switch/case for type handling.
    /// </summary>
    public partial class AddFigureDialog : Window
    {
        public IFigure? CreatedFigure { get; private set; }

        public AddFigureDialog()
        {
            InitializeComponent();
            LoadFigureTypes();
            TypeCombo_SelectionChanged(null!, null!);
        }

        private void LoadFigureTypes()
        {
            foreach (var name in FigureFactory.GetRegisteredTypes())
            {
                TypeCombo.Items.Add(name);
            }
            if (TypeCombo.Items.Count > 0)
                TypeCombo.SelectedIndex = 0;
        }

        private void TypeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs? e)
        {
            var type = TypeCombo.SelectedItem?.ToString() ?? "line";
            var order = FigureFactory.GetParamOrder(type);
            if (order == null) return;

            // First two are always x, y. Rest go to P1, P2.
            LblP1.Text = order.Length > 2 ? Capitalize(order[2]) : "";
            LblP2.Text = order.Length > 3 ? Capitalize(order[3]) : "";
            LblP2.Visibility = order.Length > 3 ? Visibility.Visible : Visibility.Collapsed;
            TxtP2.Visibility = LblP2.Visibility;
        }

        private static string Capitalize(string s) => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s[1..];

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var type = TypeCombo.SelectedItem?.ToString() ?? "line";
            var order = FigureFactory.GetParamOrder(type);
            if (order == null || order.Length < 2)
            {
                StatusText.Text = "Invalid figure type.";
                return;
            }

            var parameters = new Dictionary<string, double>();
            var values = new[] { TxtX.Text, TxtY.Text, TxtP1.Text, TxtP2.Text };

            for (int i = 0; i < order.Length && i < values.Length; i++)
            {
                if (!TryParseDouble(values[i], order[i], out var v))
                {
                    StatusText.Text = $"{Capitalize(order[i])} must be a valid number.";
                    return;
                }
                parameters[order[i]] = v;
            }

            var figure = FigureFactory.Create(type, parameters);
            if (figure == null)
            {
                StatusText.Text = "Failed to create figure.";
                return;
            }

            CreatedFigure = figure;
            DialogResult = true;
            Close();
        }

        private bool TryParseDouble(string text, string name, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
