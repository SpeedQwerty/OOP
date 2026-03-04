using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab1.Figures.Base
{
    public abstract class figure
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public System.Windows.Media.Color StrokeColor { get; set; } = Colors.Black;
        public double StrokeThickness { get; set; } = 3;
        public DoubleCollection StrokeDashArray { get; set; }
        public System.Windows.Media.Color FillColor { get; set; } = Colors.Transparent;

        protected figure(double x, double y, double width, double height)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
        }


        public abstract Shape Draw();

        protected virtual void ApplyStyles(Shape shape)
        {
            shape.Fill = new SolidColorBrush(FillColor);
            shape.Stroke = new SolidColorBrush(StrokeColor); 
            shape.StrokeThickness = StrokeThickness;    
              if (StrokeDashArray != null)
            {
                shape.StrokeDashArray = StrokeDashArray;
            }
            

        } 
          



    }

}
