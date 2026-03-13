using System.Windows.Controls;
using System.Windows.Shapes;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Rendering
{
    /// <summary>
    /// Renders EllipseFigure to WPF Ellipse shape.
    /// </summary>
    public class EllipseFigureRenderStrategy : IFigureRenderStrategy
    {
        public System.Type FigureType => typeof(Figure_graphics.Figures.EllipseFigure);

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
