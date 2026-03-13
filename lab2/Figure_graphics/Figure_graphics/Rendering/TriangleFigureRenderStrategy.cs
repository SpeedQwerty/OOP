using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Rendering
{
    /// <summary>
    /// Renders TriangleFigure to WPF Polygon shape.
    /// </summary>
    public class TriangleFigureRenderStrategy : IFigureRenderStrategy
    {
        public System.Type FigureType => typeof(Figure_graphics.Figures.TriangleFigure);

        public Shape Render(IFigure figure)
        {
            var polygon = new Polygon();
            var points = new PointCollection
            {
                new Point(figure.X + figure.Width / 2, figure.Y),
                new Point(figure.X + figure.Width, figure.Y + figure.Height),
                new Point(figure.X, figure.Y + figure.Height)
            };
            polygon.Points = points;
            FigureRenderer.ApplyStyles(polygon, figure);
            return polygon;
        }
    }
}
