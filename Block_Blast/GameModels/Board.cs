using Block_Blast.Models;

namespace Block_Blast.GameModels;

public class Board
{
    // ── Константы ─────────────────────────────────────────────
    public const int Rows = 8;
    public const int Cols = 8;

    // ── Свойства ──────────────────────────────────────────────
    public Cell[,] Grid { get; private set; }

    /// <summary>
    /// Событие: передаёт количество очищенных линий И их индексы.
    /// Item1 = count, Item2 = список (isRow, index).
    /// </summary>
    public event Action<int, List<(bool isRow, int index)>>? LinesCleared;

    // ── Конструктор ───────────────────────────────────────────
    public Board()
    {
        Grid = new Cell[Rows, Cols];
        InitGrid();
    }

    // ── Инициализация ─────────────────────────────────────────
    private void InitGrid()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                Grid[r, c] = new Cell(r, c);
    }

    // ── Основные методы ───────────────────────────────────────

    public bool CanPlace(Block block, int startRow, int startCol)
    {
        foreach (var (dr, dc) in block.GetCells())
        {
            int r = startRow + dr;
            int c = startCol + dc;
            if (r < 0 || r >= Rows || c < 0 || c >= Cols) return false;
            if (Grid[r, c].IsOccupied) return false;
        }
        return true;
    }

    public void PlaceBlock(Block block, int startRow, int startCol)
    {
        foreach (var (dr, dc) in block.GetCells())
        {
            int r = startRow + dr;
            int c = startCol + dc;
            Grid[r, c].Fill(block.BlockColor);
        }

        var (count, lines) = ClearFullLines();
        if (count > 0)
            LinesCleared?.Invoke(count, lines);
    }

    // ── Сложный режим: случайное заполнение ───────────────────

    /// <summary>
    /// Заполняет ~count случайных клеток серым цветом.
    /// Гарантирует что поле не окажется сразу заблокированным.
    /// </summary>
    public void FillRandom(int count = 12)
    {
        var rng = new Random();
        var positions = new List<(int r, int c)>();

        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                positions.Add((r, c));

        // Перемешиваем Fisher-Yates
        for (int i = positions.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (positions[i], positions[j]) = (positions[j], positions[i]);
        }

        var hardColor = Color.FromArgb("#607D8B"); // серо-синий

        int placed = 0;
        foreach (var (r, c) in positions)
        {
            if (placed >= count) break;
            // Не занимаем клетки в одном полностью — иначе сразу очистится
            Grid[r, c].Fill(hardColor);
            placed++;
        }
    }

    // ── Превью размещения ─────────────────────────────────────

    /// <summary>
    /// Возвращает список клеток (row, col) где блок окажется при размещении.
    /// Возвращает пустой список если размещение невозможно.
    /// </summary>
    public List<(int row, int col)> GetPreviewCells(Block block, int startRow, int startCol)
    {
        var cells = new List<(int, int)>();
        foreach (var (dr, dc) in block.GetCells())
        {
            int r = startRow + dr;
            int c = startCol + dc;
            if (r < 0 || r >= Rows || c < 0 || c >= Cols) return new List<(int, int)>();
            if (Grid[r, c].IsOccupied) return new List<(int, int)>();
            cells.Add((r, c));
        }
        return cells;
    }

    // ── Проверка конца игры ───────────────────────────────────

    public bool IsGameOver(List<Block> nextBlocks)
    {
        foreach (var block in nextBlocks)
            if (HasAnyValidPosition(block))
                return false;
        return true;
    }

    private bool HasAnyValidPosition(Block block)
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (CanPlace(block, r, c))
                    return true;
        return false;
    }

    /// <summary>Проверяет можно ли вообще поставить блок хоть куда-то.</summary>
    public bool CanPlaceAnywhere(Block block) => HasAnyValidPosition(block);

    // ── Очистка линий ─────────────────────────────────────────

    private (int count, List<(bool isRow, int index)> lines) ClearFullLines()
    {
        int count = 0;
        var lines = new List<(bool isRow, int index)>();

        for (int r = 0; r < Rows; r++)
        {
            if (IsRowFull(r))
            {
                lines.Add((true, r));
                count++;
            }
        }

        for (int c = 0; c < Cols; c++)
        {
            if (IsColFull(c))
            {
                lines.Add((false, c));
                count++;
            }
        }

        // Сначала собираем все линии, потом очищаем (избегаем двойной очистки)
        foreach (var (isRow, idx) in lines)
        {
            if (isRow) ClearRow(idx);
            else ClearCol(idx);
        }

        return (count, lines);
    }

    private bool IsRowFull(int row)
    {
        for (int c = 0; c < Cols; c++)
            if (!Grid[row, c].IsOccupied) return false;
        return true;
    }

    private bool IsColFull(int col)
    {
        for (int r = 0; r < Rows; r++)
            if (!Grid[r, col].IsOccupied) return false;
        return true;
    }

    private void ClearRow(int row)
    {
        for (int c = 0; c < Cols; c++) Grid[row, c].Clear();
    }

    private void ClearCol(int col)
    {
        for (int r = 0; r < Rows; r++) Grid[r, col].Clear();
    }

    // ── Сброс ─────────────────────────────────────────────────
    public void Reset() => InitGrid();
}