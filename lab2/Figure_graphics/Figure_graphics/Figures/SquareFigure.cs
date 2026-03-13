using System.Collections.Generic;
using Figure_graphics.Factory;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Figures
{
    /// <summary>
    /// Represents a square with top-left corner at (X,Y) and given side length.
    /// Does not contain drawing logic - use FigureRenderer.
    /// </summary>
    public class SquareFigure : Figure
    {
        public SquareFigure(double x, double y, double side)
            : base(x, y, side, side)
        {
        }

        public static void RegisterCreator()
        {
            FigureFactory.Register("square", p => new SquareFigure(
                FigureFactory.GetParam(p, "x", 0), FigureFactory.GetParam(p, "y", 0),
                FigureFactory.GetParam(p, "side", 50)));
            FigureFactory.RegisterParamOrder("square", new[] { "x", "y", "side" });
        }
    }
}
