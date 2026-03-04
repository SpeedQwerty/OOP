using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using Figure_graphics.Figure;
using lab1.Figures.Base;

namespace Figure_graphics.Figure
{
    public class CircleFigure:figure
    {
        public CircleFigure(double x, double y, double radius):base(x, y, radius*2, radius * 2) { }
        public override Shape Draw()
        {
            var circle = new System.Windows.Shapes.Ellipse { 
                Width = Width,
                Height= Height
            };
            ApplyStyles(circle);
            Canvas.SetLeft(circle, X); 
            Canvas.SetTop(circle, Y);
            return circle;
        }
    }
}
