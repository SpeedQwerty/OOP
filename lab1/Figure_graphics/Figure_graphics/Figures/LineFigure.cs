using lab1.Figures.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Figure_graphics.Figure;
using System.Windows.Shapes;

namespace Figure_graphics.Figure
{
    public class LineFigure : figure
    {
        public double X2 { get; set; }
        public double Y2 { get; set; }

        public LineFigure (double x1, double y1, double x2, double y2):base(x1, y1, Math.Abs(x2-x1), Math.Abs(y2-y1))
        {
            X2 = x2;
            Y2 = y2;
        }

        public override Shape Draw()
        {
            var line = new Line
            {
                X2 = X2, X1=X, Y1=Y,Y2 = Y2  
            };  
            ApplyStyles(line);
            return line; 
        }
        
    }

}
