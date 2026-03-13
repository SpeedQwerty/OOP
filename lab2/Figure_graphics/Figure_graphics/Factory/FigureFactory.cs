using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Figure_graphics.Figures.Base;

using BaseFigure = Figure_graphics.Figures.Base.Figure;

namespace Figure_graphics.Factory
{
    /// <summary>
    /// Creates figures from parameters using a registry pattern.
    /// Each figure type registers itself - adding new figure classes requires
    /// no changes to existing code (no switch/case or multiple if).
    /// </summary>
    public static class FigureFactory
    {
        public delegate IFigure FigureCreator(Dictionary<string, double> parameters);

        private static readonly Dictionary<string, FigureCreator> _creators = new();
        private static bool _initialized;

        /// <summary>
        /// Registers a figure type. Called from each figure's static Register method.
        /// </summary>
        public static void Register(string typeName, FigureCreator creator)
        {
            _creators[typeName.ToLowerInvariant()] = creator;
        }

        /// <summary>
        /// Discovers and invokes Register() on all figure types. No central switch.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (_initialized) return;

            var figureTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace == "Figure_graphics.Figures"
                            && t.IsClass
                            && !t.IsAbstract
                            && t != typeof(BaseFigure)
                            && typeof(BaseFigure).IsAssignableFrom(t));

            foreach (var type in figureTypes)
            {
                var method = type.GetMethod("RegisterCreator", BindingFlags.Public | BindingFlags.Static);
                method?.Invoke(null, null);
            }

            _initialized = true;
        }

        /// <summary>
        /// Gets all registered figure type names.
        /// </summary>
        public static IReadOnlyCollection<string> GetRegisteredTypes()
        {
            EnsureInitialized();
            return _creators.Keys;
        }

        /// <summary>
        /// Creates a figure from type name and parameters.
        /// </summary>
        public static IFigure? Create(string typeName, Dictionary<string, double> parameters)
        {
            EnsureInitialized();
            var key = typeName.ToLowerInvariant();
            if (_creators.TryGetValue(key, out var creator))
            {
                return creator(parameters);
            }
            return null;
        }

        /// <summary>
        /// Helper to get parameter value with default.
        /// </summary>
        public static double GetParam(Dictionary<string, double> p, string key, double defaultValue)
        {
            return p.TryGetValue(key, out var v) ? v : defaultValue;
        }

        /// <summary>
        /// Parameter order for dialogs. Each figure registers this in RegisterCreator.
        /// </summary>
        private static readonly Dictionary<string, string[]> _paramOrder = new();

        public static void RegisterParamOrder(string typeName, string[] paramNames)
        {
            _paramOrder[typeName.ToLowerInvariant()] = paramNames;
        }

        /// <summary>
        /// Gets parameter names for a type (e.g. line -> x, y, x2, y2).
        /// </summary>
        public static string[]? GetParamOrder(string typeName)
        {
            EnsureInitialized();
            return _paramOrder.TryGetValue(typeName.ToLowerInvariant(), out var order) ? order : null;
        }
    }
}
