using System;
using System.Collections.Generic;
using System.Text;

namespace Block_Blast.GameModels
{
    public class Cell
    {
        public int Row { get; private set; }
        public int Col { get; private set; }
        public bool IsOccupied { get; private set; }
        public Color CellColor { get; private set; }

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
            IsOccupied = false;
            CellColor = Colors.Transparent;
        }
        public void Fill(Color color)
        {
            IsOccupied = true;
            CellColor = color;
        }
        public void Clear()
        {
            IsOccupied = false;
            CellColor = Colors.Transparent;
        }

    }
}
