using System.Windows.Media;

namespace Figure_graphics.Figures.Base
{
    /// <summary>
    /// Base class for all graphic figures. Contains only data - no drawing methods.
    /// Rendering is delegated to FigureRenderer via IFigureRenderStrategy pattern.
    /// </summary>
    public abstract class Figure : IFigure
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Color StrokeColor { get; set; } = Colors.Black;
        public double StrokeThickness { get; set; } = 2;
        public DoubleCollection? StrokeDashArray { get; set; }
        public Color FillColor { get; set; } = Colors.Transparent;

        protected Figure(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
