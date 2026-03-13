using Figure_graphics.Figure;
using Figure_graphics.Figures;
using System.Windows;
using System.Windows.Media;

namespace Figure_graphics
{
    public partial class MainWindow : Window
    {
        private FigureList? _figureList; // Делаем поле nullable

        public MainWindow()
        {
            InitializeComponent();
            InitializeFigures();
        }

        private void InitializeFigures()
        {
            _figureList = new FigureList();

            // 1. Отрезок (линия)
            var line = new LineFigure(50, 50, 200, 50)
            {
                StrokeColor = Colors.Red,
                StrokeThickness = 3
            };
            _figureList.AddFigure(line);

            // 2. Прямоугольник
            var rectangle = new RectangleFigure(50, 100, 150, 80)
            {
                StrokeColor = Colors.Blue,
                FillColor = Colors.LightBlue,
                StrokeThickness = 2
            };
            _figureList.AddFigure(rectangle);

            // 3. Эллипс
            var ellipse = new EllipseFigure(250, 100, 120, 80)
            {
                StrokeColor = Colors.Green,
                FillColor = Colors.LightGreen,
                StrokeThickness = 2
            };
            _figureList.AddFigure(ellipse);

            // 4. Треугольник
            var triangle = new TriangleFigure(420, 100, 100, 80)
            {
                StrokeColor = Colors.Orange,
                FillColor = Colors.LightSalmon,
                StrokeThickness = 2
            };
            _figureList.AddFigure(triangle);

            // 5. Круг
            var circle = new CircleFigure(570, 120, 40)
            {
                StrokeColor = Colors.Purple,
                FillColor = Colors.Lavender,
                StrokeThickness = 2
            };
            _figureList.AddFigure(circle);

            // 6. Квадрат - исправленный конструктор
            var square = new SquareFigure(650, 100, 70)
            {
                StrokeColor = Colors.Brown,
                FillColor = Colors.SandyBrown,
                StrokeThickness = 2
            };
            _figureList.AddFigure(square);

            // Дополнительные фигуры
            var line2 = new LineFigure(50, 250, 250, 350)
            {
                StrokeColor = Colors.Cyan,
                StrokeThickness = 4,
                StrokeDashArray = new DoubleCollection { 5, 2 }
            };
            _figureList.AddFigure(line2);

            var ellipse2 = new EllipseFigure(300, 250, 150, 60)
            {
                StrokeColor = Colors.Magenta,
                FillColor = Colors.LightPink,
                StrokeThickness = 3
            };
            _figureList.AddFigure(ellipse2);

            var rectangle2 = new RectangleFigure(500, 250, 120, 100)
            {
                StrokeColor = Colors.DarkGreen,
                FillColor = Colors.LightGreen,
                StrokeThickness = 2
            };
            _figureList.AddFigure(rectangle2);
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что Canvas и список фигур существуют
            if (DrawingCanvas == null || _figureList == null)
                return;

            // Очищаем Canvas
            DrawingCanvas.Children.Clear();

            // Рисуем все фигуры из списка
            foreach (var figure in _figureList.GetAllFigures())
            {
                var shape = figure.Draw();
                DrawingCanvas.Children.Add(shape);
            }
        }
    }
}