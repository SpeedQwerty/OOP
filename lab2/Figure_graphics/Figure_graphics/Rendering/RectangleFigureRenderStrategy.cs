using System.Windows.Controls;
using System.Windows.Shapes;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Rendering
{
    /// <summary>
    /// Renders RectangleFigure to WPF Rectangle shape.
    /// </summary>
    public class RectangleFigureRenderStrategy : IFigureRenderStrategy
    {
        public System.Type FigureType => typeof(Figure_graphics.Figures.RectangleFigure);

        public Shape Render(IFigure figure)
        {
            var rect = new Rectangle
            {
                Width = figure.Width,
                Height = figure.Height
            };
            FigureRenderer.ApplyStyles(rect, figure);
            System.Windows.Controls.Canvas.SetLeft(rect, figure.X);
            System.Windows.Controls.Canvas.SetTop(rect, figure.Y);
            return rect;
        }
    }
}
