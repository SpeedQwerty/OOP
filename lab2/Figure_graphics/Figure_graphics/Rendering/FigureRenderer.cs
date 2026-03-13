using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Shapes;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Rendering
{
    /// <summary>
    /// Renders figures to WPF Shapes. Uses strategy pattern with auto-discovery -
    /// new figure types are supported by adding new IFigureRenderStrategy implementations.
    /// No modifications to existing code required when adding new figures.
    /// </summary>
    public static class FigureRenderer
    {
        private static readonly Dictionary<Type, IFigureRenderStrategy> _strategies = new();
        private static bool _initialized;

        /// <summary>
        /// Applies stroke and fill styles to a WPF Shape from figure data.
        /// </summary>
        public static void ApplyStyles(System.Windows.Shapes.Shape shape, IFigure figure)
        {
            shape.Fill = new System.Windows.Media.SolidColorBrush(figure.FillColor);
            shape.Stroke = new System.Windows.Media.SolidColorBrush(figure.StrokeColor);
            shape.StrokeThickness = figure.StrokeThickness;
            if (figure.StrokeDashArray != null)
            {
                shape.StrokeDashArray = figure.StrokeDashArray;
            }
        }

        /// <summary>
        /// Ensures all render strategies are discovered and registered.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (_initialized) return;

            var strategyTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IFigureRenderStrategy).IsAssignableFrom(t)
                            && !t.IsAbstract
                            && !t.IsInterface);

            foreach (var type in strategyTypes)
            {
                var instance = (IFigureRenderStrategy?)Activator.CreateInstance(type);
                if (instance != null)
                {
                    _strategies[instance.FigureType] = instance;
                }
            }

            _initialized = true;
        }

        /// <summary>
        /// Renders a figure to a WPF Shape. Uses registered strategy for the figure type.
        /// </summary>
        /// <param name="figure">The figure to render</param>
        /// <returns>WPF Shape ready to add to Canvas, or null if no strategy found</returns>
        public static Shape? Render(IFigure figure)
        {
            if (figure == null) return null;

            EnsureInitialized();

            var type = figure.GetType();
            if (_strategies.TryGetValue(type, out var strategy))
            {
                return strategy.Render(figure);
            }

            return null;
        }
    }
}
