using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Figure_graphics.Dialogs;
using Figure_graphics.Factory;
using Figure_graphics.Figures;
using Figure_graphics.Figures.Base;
using Figure_graphics.Input;
using Figure_graphics.Rendering;

namespace Figure_graphics
{
    /// <summary>
    /// Main window of the primitive graphic editor.
    /// Supports three input methods: Mouse, Dialog, Script.
    /// </summary>
    public partial class MainWindow : Window
    {
        private FigureList _figureList = new();
        private bool _mouseMode;
        private string _mouseFigureType = "line";
        private readonly List<Point> _mousePoints = new();

        public MainWindow()
        {
            InitializeComponent();
            AddSampleFigures();
        }

        /// <summary>
        /// Adds sample figures for initial display.
        /// </summary>
        private void AddSampleFigures()
        {
            var line = new LineFigure(50, 50, 200, 50) { StrokeColor = Colors.Red, StrokeThickness = 3 };
            _figureList.AddFigure(line);

            var rect = new RectangleFigure(50, 100, 150, 80)
            {
                StrokeColor = Colors.Blue, FillColor = Colors.LightBlue, StrokeThickness = 2
            };
            _figureList.AddFigure(rect);

            var ellipse = new EllipseFigure(250, 100, 120, 80)
            {
                StrokeColor = Colors.Green, FillColor = Colors.LightGreen, StrokeThickness = 2
            };
            _figureList.AddFigure(ellipse);

            var circle = new CircleFigure(520, 120, 40)
            {
                StrokeColor = Colors.Purple, FillColor = Colors.Lavender, StrokeThickness = 2
            };
            _figureList.AddFigure(circle);

            RefreshCanvas();
        }

        /// <summary>
        /// Activates mouse input mode. User selects type via dialog, then clicks on canvas.
        /// </summary>
        private void BtnMouse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SelectFigureTypeDialog();
            if (dlg.ShowDialog() == true && dlg.SelectedType != null)
            {
                _mouseFigureType = dlg.SelectedType;
                _mousePoints.Clear();
                _mouseMode = true;
                MouseHint.Text = $"Mouse mode: {_mouseFigureType} - click on canvas (2 points)";
                BtnMouse.Foreground = Brushes.DarkGreen;
            }
        }

        /// <summary>
        /// Opens dialog for manual parameter input.
        /// </summary>
        private void BtnDialog_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddFigureDialog { Owner = this };
            if (dlg.ShowDialog() == true && dlg.CreatedFigure != null)
            {
                _figureList.AddFigure(dlg.CreatedFigure);
                RefreshCanvas();
            }
        }

        /// <summary>
        /// Expands script panel for script input.
        /// </summary>
        private void BtnScript_Click(object sender, RoutedEventArgs e)
        {
            ScriptExpander.IsExpanded = true;
        }

        /// <summary>
        /// Handles mouse clicks on canvas for mouse input mode.
        /// </summary>
        private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_mouseMode) return;

            var pos = e.GetPosition(DrawingCanvas);
            _mousePoints.Add(new Point(pos.X, pos.Y));

            int needed = GetPointsNeeded(_mouseFigureType);
            if (_mousePoints.Count >= needed)
            {
                var figure = CreateFigureFromMousePoints(_mouseFigureType, _mousePoints);
                if (figure != null)
                {
                    _figureList.AddFigure(figure);
                    RefreshCanvas();
                }
                _mousePoints.Clear();
                _mouseMode = false;
                MouseHint.Text = "";
                BtnMouse.Foreground = System.Windows.Media.Brushes.Black;
            }
            else
            {
                MouseHint.Text = $"{_mouseFigureType}: point {_mousePoints.Count + 1}/{needed}";
            }
        }

        /// <summary>
        /// Returns number of click points needed for the given figure type.
        /// </summary>
        private static int GetPointsNeeded(string type)
        {
            return type.ToLowerInvariant() switch
            {
                "line" => 2,
                "rectangle" or "ellipse" or "triangle" => 2,
                "circle" => 2,
                "square" => 2,
                _ => 2
            };
        }

        /// <summary>
        /// Creates a figure from collected mouse points. Uses FigureFactory - no switch for creation.
        /// </summary>
        private IFigure? CreateFigureFromMousePoints(string type, List<Point> points)
        {
            var p = new Dictionary<string, double>();
            var t = type.ToLowerInvariant();

            if (t == "line" && points.Count >= 2)
            {
                p["x"] = points[0].X; p["y"] = points[0].Y;
                p["x2"] = points[1].X; p["y2"] = points[1].Y;
            }
            else if ((t == "rectangle" || t == "ellipse" || t == "triangle") && points.Count >= 2)
            {
                var x1 = Math.Min(points[0].X, points[1].X);
                var y1 = Math.Min(points[0].Y, points[1].Y);
                var w = Math.Abs(points[1].X - points[0].X);
                var h = Math.Abs(points[1].Y - points[0].Y);
                if (w < 5) w = 5; if (h < 5) h = 5;
                p["x"] = x1; p["y"] = y1; p["width"] = w; p["height"] = h;
            }
            else if (t == "circle" && points.Count >= 2)
            {
                var cx = points[0].X; var cy = points[0].Y;
                var r = Math.Sqrt((points[1].X - cx) * (points[1].X - cx) + (points[1].Y - cy) * (points[1].Y - cy));
                if (r < 5) r = 5;
                p["x"] = cx - r; p["y"] = cy - r; p["radius"] = r;
            }
            else if (t == "square" && points.Count >= 2)
            {
                var side = Math.Max(
                    Math.Abs(points[1].X - points[0].X),
                    Math.Abs(points[1].Y - points[0].Y));
                if (side < 5) side = 5;
                var x1 = Math.Min(points[0].X, points[1].X);
                var y1 = Math.Min(points[0].Y, points[1].Y);
                p["x"] = x1; p["y"] = y1; p["side"] = side;
            }
            else
            {
                return null;
            }

            return FigureFactory.Create(type, p);
        }

        /// <summary>
        /// Executes script commands from the script text box.
        /// </summary>
        private void ExecuteScript_Click(object sender, RoutedEventArgs e)
        {
            var text = ScriptTextBox.Text ?? "";
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var count = 0;
            foreach (var line in lines)
            {
                var figure = ScriptParser.ParseLine(line);
                if (figure != null)
                {
                    _figureList.AddFigure(figure);
                    count++;
                }
            }
            RefreshCanvas();
            ScriptStatus.Text = count > 0 ? $"Added {count} figure(s)." : "No valid commands.";
        }

        /// <summary>
        /// Redraws all figures on the canvas using FigureRenderer.
        /// </summary>
        private void RefreshCanvas_Click(object sender, RoutedEventArgs e)
        {
            RefreshCanvas();
        }

        private void RefreshCanvas()
        {
            DrawingCanvas.Children.Clear();
            foreach (var figure in _figureList.GetAllFigures())
            {
                var shape = FigureRenderer.Render(figure);
                if (shape != null)
                    DrawingCanvas.Children.Add(shape);
            }
        }

        /// <summary>
        /// Clears all figures from the editor.
        /// </summary>
        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            _figureList.Clear();
            DrawingCanvas.Children.Clear();
            ScriptTextBox.Clear();
            ScriptStatus.Text = "";
        }
    }
}
