using System;
using System.Collections.Generic;
using System.Text;

namespace Block_Blast.GameModels
{
    public class Block
    {
        public int[,] Shape { get; private set; }   
        public Color BlockColor { get; private set; }
        public int Rows => Shape.GetLength(0);
        public int Cols => Shape.GetLength(1);

        public Block(int[,] shape, Color color)
        {
            Shape = shape;
            BlockColor = color;
        }

        public List<(int row, int col)> GetCells()
        {
            var cells = new List<(int, int)>();

            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    if (Shape[r, c] == 1)
                        cells.Add((r, c));

            return cells;
        }

        public static Block Square(Color color) => new Block(new int[,]
        {
            { 1, 1 },
            { 1, 1 }
        }, color);

        public static Block LineH(Color color) => new Block(new int[,]
        {
            { 1, 1, 1, 1 }
        }, color);

        public static Block LineV(Color color) => new Block(new int[,]
        {
            { 1 },
            { 1 },
            { 1 },
            { 1 }
        }, color);

        public static Block LineV5(Color color) => new Block(new int[,]
{
            { 1 },
            { 1 },
            { 1 },
            { 1 },
            { 1 }
}, color);

        public static Block LShape(Color color) => new Block(new int[,]
        {
            { 1, 0 },
            { 1, 0 },
            { 1, 1 }
        }, color);

        public static Block LShapeReverse(Color color) => new Block(new int[,]
        {
            { 0, 1 },
            { 0, 1 },
            { 1, 1 }
        }, color);

        public static Block SShape(Color color) => new Block(new int[,]
        {
            { 1, 1, 0 },
            { 0, 1, 1 }
        }, color);

        public static Block ZShape(Color color) => new Block(new int[,]
        {
            { 0, 1, 1 },
            { 1, 1, 0 }
        }, color);

        public static Block TShape(Color color) => new Block(new int[,]
        {
            { 1, 1, 1 },
            { 0, 1, 0 }
        }, color);

        public static Block Single(Color color) => new Block(new int[,]
        {
            { 1 }
        }, color);

        public static Block LineH3(Color color) => new Block(new int[,]
        {
            { 1, 1, 1 }
        }, color);

        public static Block LineV3(Color color) => new Block(new int[,]
        {
            { 1 },
            { 1 },
            { 1 }
        }, color);

        public static Block Square3(Color color) => new Block(new int[,]
        {
            {1, 1, 1 },
            {1, 1, 1 },
            {1, 1, 1 }
        }, color);

    }

}
