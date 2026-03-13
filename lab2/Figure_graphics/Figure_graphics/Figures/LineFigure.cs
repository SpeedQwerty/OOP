using System.Collections.Generic;
using Figure_graphics.Factory;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Figures
{
    /// <summary>
    /// Represents a line segment from (X,Y) to (X2,Y2).
    /// Does not contain drawing logic - use FigureRenderer.
    /// </summary>
    public class LineFigure : Figure
    {
        public double X2 { get; set; }
        public double Y2 { get; set; }

        public LineFigure(double x1, double y1, double x2, double y2)
            : base(x1, y1, Math.Abs(x2 - x1), Math.Abs(y2 - y1))
        {
            X2 = x2;
            Y2 = y2;
        }

        /// <summary>
        /// Registers this figure type in the factory. Called via reflection - no changes to existing code.
        /// </summary>
        public static void RegisterCreator()
        {
            FigureFactory.Register("line", p => new LineFigure(
                FigureFactory.GetParam(p, "x", 0), FigureFactory.GetParam(p, "y", 0),
                FigureFactory.GetParam(p, "x2", 100), FigureFactory.GetParam(p, "y2", 0)));
            FigureFactory.RegisterParamOrder("line", new[] { "x", "y", "x2", "y2" });
        }
    }
}
