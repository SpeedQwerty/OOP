using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using Figure_graphics.Figures;
using lab1.Figures.Base;
namespace Figure_graphics.Figures
{
    public class EllipseFigure:figure
    {
        public EllipseFigure(double x, double y, double width, double height) : base(x, y, width, height) { }
        public override Shape Draw()
        {
            var ellipse = new System.Windows.Shapes.Ellipse{ 
                Width = Width,
                Height = Height
            };
            ApplyStyles(ellipse);
            Canvas.SetLeft(ellipse, X); 
            Canvas.SetTop(ellipse, Y);
            return ellipse;
        }
    }
}

