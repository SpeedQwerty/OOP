using System.Collections.Generic;
using Figure_graphics.Factory;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Figures
{
    /// <summary>
    /// Represents a circle with center at (X+radius, Y+radius) and given radius.
    /// Stored as width=height=2*radius. Does not contain drawing logic - use FigureRenderer.
    /// </summary>
    public class CircleFigure : Figure
    {
        public double Radius => Width / 2;

        public CircleFigure(double x, double y, double radius)
            : base(x, y, radius * 2, radius * 2)
        {
        }

        public static void RegisterCreator()
        {
            FigureFactory.Register("circle", p => new CircleFigure(
                FigureFactory.GetParam(p, "x", 0), FigureFactory.GetParam(p, "y", 0),
                FigureFactory.GetParam(p, "radius", 40)));
            FigureFactory.RegisterParamOrder("circle", new[] { "x", "y", "radius" });
        }
    }
}
