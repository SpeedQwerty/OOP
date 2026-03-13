using System.Collections.Generic;
using Figure_graphics.Factory;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Figures
{
    /// <summary>
    /// Represents a rectangle with top-left corner at (X,Y) and given width/height.
    /// Does not contain drawing logic - use FigureRenderer.
    /// </summary>
    public class RectangleFigure : Figure
    {
        public RectangleFigure(double x, double y, double width, double height)
            : base(x, y, width, height)
        {
        }

        public static void RegisterCreator()
        {
            FigureFactory.Register("rectangle", p => new RectangleFigure(
                FigureFactory.GetParam(p, "x", 0), FigureFactory.GetParam(p, "y", 0),
                FigureFactory.GetParam(p, "width", 100), FigureFactory.GetParam(p, "height", 80)));
            FigureFactory.RegisterParamOrder("rectangle", new[] { "x", "y", "width", "height" });
        }
    }
}
