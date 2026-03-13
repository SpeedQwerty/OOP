using System.Windows.Shapes;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Rendering
{
    /// <summary>
    /// Strategy interface for rendering a specific figure type to WPF Shape.
    /// New figure types add new strategy implementations - no changes to existing code needed.
    /// </summary>
    public interface IFigureRenderStrategy
    {
        /// <summary>
        /// The figure type this strategy can render.
        /// </summary>
        System.Type FigureType { get; }

        /// <summary>
        /// Creates a WPF Shape from the given figure data.
        /// </summary>
        Shape Render(IFigure figure);
    }
}
