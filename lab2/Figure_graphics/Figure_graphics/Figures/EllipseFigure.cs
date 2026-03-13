using System.Collections.Generic;
using Figure_graphics.Factory;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Figures
{
    /// <summary>
    /// Represents an ellipse with bounding box at (X,Y) and given width/height.
    /// Does not contain drawing logic - use FigureRenderer.
    /// </summary>
    public class EllipseFigure : Figure
    {
        public EllipseFigure(double x, double y, double width, double height)
            : base(x, y, width, height)
        {
        }

        public static void RegisterCreator()
        {
            FigureFactory.Register("ellipse", p => new EllipseFigure(
                FigureFactory.GetParam(p, "x", 0), FigureFactory.GetParam(p, "y", 0),
                FigureFactory.GetParam(p, "width", 100), FigureFactory.GetParam(p, "height", 80)));
            FigureFactory.RegisterParamOrder("ellipse", new[] { "x", "y", "width", "height" });
        }
    }
}
