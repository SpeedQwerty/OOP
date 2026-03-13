using System.Windows.Controls;
using System.Windows.Shapes;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Rendering
{
    /// <summary>
    /// Renders SquareFigure to WPF Rectangle shape.
    /// </summary>
    public class SquareFigureRenderStrategy : IFigureRenderStrategy
    {
        public System.Type FigureType => typeof(Figure_graphics.Figures.SquareFigure);

        public Shape Render(IFigure figure)
        {
            var rect = new Rectangle
            {
                Width = figure.Width,
                Height = figure.Height
            };
            FigureRenderer.ApplyStyles(rect, figure);
            Canvas.SetLeft(rect, figure.X);
            Canvas.SetTop(rect, figure.Y);
            return rect;
        }
    }
}
