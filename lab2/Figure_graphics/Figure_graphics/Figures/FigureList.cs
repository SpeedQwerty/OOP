using System.Collections.Generic;
using System.Collections.ObjectModel;
using Figure_graphics.Figures.Base;

namespace Figure_graphics.Figures
{
    /// <summary>
    /// Manages a collection of figures for the graphic editor.
    /// </summary>
    public class FigureList
    {
        private readonly ObservableCollection<IFigure> _figures = new();

        public void AddFigure(IFigure figure)
        {
            _figures.Add(figure);
        }

        public void RemoveFigure(IFigure figure)
        {
            _figures.Remove(figure);
        }

        public IReadOnlyList<IFigure> GetAllFigures()
        {
            return _figures;
        }

        public void Clear()
        {
            _figures.Clear();
        }

        public int Count => _figures.Count;
    }
}
