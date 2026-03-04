using System;
using System.Collections.Generic;
using System.Text;
using System.Windows; 
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Figure_graphics.Figures;

using lab1.Figures.Base;

namespace Figure_graphics.Figures
{
    public class TriangleFigure:figure
    {
        public TriangleFigure(double x, double y, double width, double height) : base(x, y, width, height) { }
        public override Shape Draw()
        {
            var polygon = new Polygon();
            PointCollection points = new PointCollection();
            points.Add(new Point(X + Width / 2, Y));
            points.Add(new Point(X + Width, Y + Height)); 
            points.Add(new Point(X, Y+ Height));
            polygon.Points = points;
            ApplyStyles(polygon); 
            return polygon;
        }
    }
}
