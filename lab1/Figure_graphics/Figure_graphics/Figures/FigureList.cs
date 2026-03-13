using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Figure_graphics.Figures;
using lab1.Figures.Base;

namespace Figure_graphics.Figures
{
    public class FigureList
    {
        private ObservableCollection<figure> figures;
        public FigureList() {
            figures = new ObservableCollection<figure>();
        }
        public void AddFigure(figure figure)
        {
            figures.Add(figure);
        }

        public void RemoveFigure(figure figure)
        {
            figures.Remove(figure);
        }
        public ObservableCollection<figure> GetAllFigures() {
            return figures;
        }
        public void Clear() { 
            figures.Clear();
        }

        public int Count => figures.Count; 
        
    }
}
