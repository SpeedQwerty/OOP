using lab1.Figures.Base;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Figure_graphics.Figures
{
    public class SquareFigure : figure
    {
        public SquareFigure(double x, double y, double side)
            : base(x, y, side, side)
        {
        }

        public override Shape Draw()
        {
            var square = new System.Windows.Shapes.Rectangle
            {
                Width = Width,
                Height = Height
            };

            ApplyStyles(square); // Применяем стили из базового класса
            Canvas.SetLeft(square, X);
            Canvas.SetTop(square, Y);

            return square;
        }
    }
}