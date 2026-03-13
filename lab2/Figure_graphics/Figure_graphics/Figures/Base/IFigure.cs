using System.Windows.Media;

namespace Figure_graphics.Figures.Base
{
    /// <summary>
    /// Interface for all graphic figures. Contains only data properties.
    /// Drawing is performed by FigureRenderer - figure classes do not contain drawing logic.
    /// </summary>
    public interface IFigure
    {
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        Color StrokeColor { get; set; }
        double StrokeThickness { get; set; }
        DoubleCollection? StrokeDashArray { get; set; }
        Color FillColor { get; set; }
    }
}
