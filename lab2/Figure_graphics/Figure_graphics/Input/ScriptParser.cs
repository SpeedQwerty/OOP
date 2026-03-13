using System;
using System.Collections.Generic;
using System.Globalization;
using Figure_graphics.Factory;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Input
{
    /// <summary>
    /// Parses script commands to create figures.
    /// Format: type param1=val1 param2=val2 ... (or shorthand: type val1 val2 ... for known types)
    /// Named: line x=10 y=20 x2=100 y2=50
    /// Positional: line 10 20 100 50 (x y x2 y2 for line)
    /// </summary>
    public static class ScriptParser
    {
        /// <summary>
        /// Parses a script line and creates a figure. Uses FigureFactory.GetParamOrder - no switch.
        /// Supports named (x=10) and positional (10 20 30 40) params.
        /// </summary>
        public static IFigure? ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;

            var parts = line.Trim().Split((char[])[' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return null;

            var typeName = parts[0].ToLowerInvariant();
            var parameters = new Dictionary<string, double>();

            // Check if first arg contains '=' (named params)
            if (parts[1].Contains('='))
            {
                for (int i = 1; i < parts.Length; i++)
                {
                    var kv = parts[i].Split('=');
                    if (kv.Length == 2 && double.TryParse(kv[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                    {
                        parameters[kv[0].ToLowerInvariant()] = v;
                    }
                }
            }
            else
            {
                var order = FigureFactory.GetParamOrder(typeName);
                if (order == null)
                    return null;
                for (int i = 1; i < parts.Length && i - 1 < order.Length; i++)
                {
                    if (double.TryParse(parts[i], NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                    {
                        parameters[order[i - 1]] = v;
                    }
                }
            }

            return parameters.Count > 0 ? FigureFactory.Create(typeName, parameters) : null;
        }

        /// <summary>
        /// Returns help text describing the script format.
        /// </summary>
        public static string GetHelp()
        {
            return @"Script format (one command per line):
  line x y x2 y2
  rectangle x y width height
  ellipse x y width height
  circle x y radius
  triangle x y width height
  square x y side";
        }
    }
}
