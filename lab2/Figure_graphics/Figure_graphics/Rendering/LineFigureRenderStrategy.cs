using System.Windows;
using System.Windows.Shapes;
using Figure_graphics.Figures;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Rendering
{
    /// <summary>
    /// Renders LineFigure to WPF Line shape.
    /// </summary>
    public class LineFigureRenderStrategy : IFigureRenderStrategy
    {
        public System.Type FigureType => typeof(LineFigure);

        public Shape Render(IFigure figure)
        {
            var lineFig = (LineFigure)figure;
            var line = new Line
            {
                X1 = lineFig.X,
                Y1 = lineFig.Y,
                X2 = lineFig.X2,
                Y2 = lineFig.Y2
            };
            FigureRenderer.ApplyStyles(line, figure);
            return line;
        }
    }
}
