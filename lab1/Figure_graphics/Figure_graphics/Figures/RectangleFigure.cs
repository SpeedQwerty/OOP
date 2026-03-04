using lab1.Figures.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Figure_graphics.Figures
{
    public class RectangleFigure:figure
    {
        public RectangleFigure(double x, double y, double width, double height) : base(x, y, width, height) { }
        public override Shape Draw()
        {
            var rectangle = new System.Windows.Shapes.Rectangle { 
                Width = Width, Height=Height
            };
            ApplyStyles(rectangle);
            Canvas.SetLeft(rectangle, X);
            Canvas.SetTop(rectangle, Y);
            return rectangle;
        }

    }
        
}
