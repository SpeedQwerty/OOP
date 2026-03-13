using System.Windows.Controls;
using System.Windows.Shapes;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Rendering
{
    /// <summary>
    /// Renders CircleFigure to WPF Ellipse shape (circle = equal width/height).
    /// </summary>
    public class CircleFigureRenderStrategy : IFigureRenderStrategy
    {
        public System.Type FigureType => typeof(Figure_graphics.Figures.CircleFigure);

        public Shape Render(IFigure figure)
        {
            var ellipse = new Ellipse
            {
                Width = figure.Width,
                Height = figure.Height
            };
            FigureRenderer.ApplyStyles(ellipse, figure);
            Canvas.SetLeft(ellipse, figure.X);
            Canvas.SetTop(ellipse, figure.Y);
            return ellipse;
        }
    }
}
