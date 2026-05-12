using System;
using System.Collections.Generic;
using System.Text;

namespace Block_Blast.GameModels
{
    public class Board
    {
        // ── Константы ─────────────────────────────────────────
        public const int Rows = 8;
        public const int Cols = 8;

        // ── Свойства ──────────────────────────────────────────
        public Cell[,] Grid { get; private set; }

        // Событие — сколько линий очищено за один ход (для подсчёта очков)
        public event Action<int>? LinesCleared;

        // ── Конструктор ───────────────────────────────────────
        public Board()
        {
            Grid = new Cell[Rows, Cols];
            InitGrid();
        }

        // ── Инициализация ─────────────────────────────────────
        private void InitGrid()
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    Grid[r, c] = new Cell(r, c);
        }

        // ── Основные методы ───────────────────────────────────

        // Проверяет, можно ли разместить блок начиная с позиции (startRow, startCol)
        public bool CanPlace(Block block, int startRow, int startCol)
        {
            foreach (var (dr, dc) in block.GetCells())
            {
                int r = startRow + dr;
                int c = startCol + dc;

                // выходит за пределы поля
                if (r < 0 || r >= Rows || c < 0 || c >= Cols)
                    return false;

                // клетка уже занята
                if (Grid[r, c].IsOccupied)
                    return false;
            }
            return true;
        }

        // Размещает блок на поле и очищает заполненные линии
        public void PlaceBlock(Block block, int startRow, int startCol)
        {
            // Заполняем клетки
            foreach (var (dr, dc) in block.GetCells())
            {
                int r = startRow + dr;
                int c = startCol + dc;
                Grid[r, c].Fill(block.BlockColor);
            }

            // Проверяем и очищаем линии
            int cleared = ClearFullLines();

            if (cleared > 0)
                LinesCleared?.Invoke(cleared);
        }

        // Очищает все заполненные строки и столбцы, возвращает количество очищенных
        private int ClearFullLines()
        {
            int count = 0;

            // Проверяем строки
            for (int r = 0; r < Rows; r++)
            {
                if (IsRowFull(r))
                {
                    ClearRow(r);
                    count++;
                }
            }

            // Проверяем столбцы
            for (int c = 0; c < Cols; c++)
            {
                if (IsColFull(c))
                {
                    ClearCol(c);
                    count++;
                }
            }

            return count;
        }

        // ── Проверки ──────────────────────────────────────────

        // Проверяет заполнена ли строка целиком
        private bool IsRowFull(int row)
        {
            for (int c = 0; c < Cols; c++)
                if (!Grid[row, c].IsOccupied)
                    return false;
            return true;
        }

        // Проверяет заполнен ли столбец целиком
        private bool IsColFull(int col)
        {
            for (int r = 0; r < Rows; r++)
                if (!Grid[r, col].IsOccupied)
                    return false;
            return true;
        }

        // ── Очистка ───────────────────────────────────────────

        private void ClearRow(int row)
        {
            for (int c = 0; c < Cols; c++)
                Grid[row, c].Clear();
        }

        private void ClearCol(int col)
        {
            for (int r = 0; r < Rows; r++)
                Grid[r, col].Clear();
        }

        // ── Проверка конца игры ───────────────────────────────

        // Возвращает true если ни один из трёх блоков не помещается на поле
        public bool IsGameOver(List<Block> nextBlocks)
        {
            foreach (var block in nextBlocks)
                if (HasAnyValidPosition(block))
                    return false;

            return true;
        }

        // Проверяет есть ли хоть одна позиция где блок помещается
        private bool HasAnyValidPosition(Block block)
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    if (CanPlace(block, r, c))
                        return true;

            return false;
        }

        // ── Сброс поля ────────────────────────────────────────
        public void Reset()
        {
            InitGrid();
        }
    }

}
