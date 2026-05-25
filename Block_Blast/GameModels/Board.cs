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
    /// Событие после очистки линий.
    /// int  = кол-во линий
    /// List = список (isRow, index) — какие именно линии
    /// bool = isBoardClear — поле полностью пустое?
    /// </summary>
    public event Action<int, List<(bool isRow, int index)>, bool>? LinesCleared;

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
        {
            bool boardClear = IsBoardEmpty();
            LinesCleared?.Invoke(count, lines, boardClear);
        }
    }

    // ── Board clear ───────────────────────────────────────────

    /// <summary>True если на поле нет ни одной занятой клетки.</summary>
    public bool IsBoardEmpty()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (Grid[r, c].IsOccupied) return false;
        return true;
    }

    // ── Сложный режим ─────────────────────────────────────────

    public void FillRandom(int count = 12)
    {
        var rng = new Random();
        var positions = new List<(int r, int c)>();

        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                positions.Add((r, c));

        for (int i = positions.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (positions[i], positions[j]) = (positions[j], positions[i]);
        }

        var hardColor = Color.FromArgb("#607D8B");
        int placed = 0;
        foreach (var (r, c) in positions)
        {
            if (placed >= count) break;
            Grid[r, c].Fill(hardColor);
            placed++;
        }
    }

    // ── Превью ────────────────────────────────────────────────

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

    /// <summary>
    /// Симулирует размещение блока и возвращает список линий которые будут очищены.
    /// Не меняет реальное состояние поля.
    /// Возвращает пустой список если размещение невозможно или линий нет.
    /// </summary>
    public List<(bool isRow, int index)> GetWouldClearLines(Block block, int startRow, int startCol)
    {
        var result = new List<(bool isRow, int index)>();

        // Проверяем что можно поставить
        var placeCells = GetPreviewCells(block, startRow, startCol);
        if (placeCells.Count == 0) return result;

        // Симулируем: набор занятых клеток = текущие + блок
        var occupied = new HashSet<(int r, int c)>();
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (Grid[r, c].IsOccupied) occupied.Add((r, c));

        foreach (var (r, c) in placeCells)
            occupied.Add((r, c));

        // Проверяем строки
        for (int r = 0; r < Rows; r++)
        {
            bool full = true;
            for (int c = 0; c < Cols; c++)
                if (!occupied.Contains((r, c))) { full = false; break; }
            if (full) result.Add((true, r));
        }

        // Проверяем столбцы
        for (int c = 0; c < Cols; c++)
        {
            bool full = true;
            for (int r = 0; r < Rows; r++)
                if (!occupied.Contains((r, c))) { full = false; break; }
            if (full) result.Add((false, c));
        }

        return result;
    }

    public bool CanPlaceAnywhere(Block block)
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (CanPlace(block, r, c)) return true;
        return false;
    }

    // ── Конец игры ────────────────────────────────────────────

    public bool IsGameOver(List<Block> nextBlocks)
    {
        foreach (var block in nextBlocks)
            if (CanPlaceAnywhere(block)) return false;
        return true;
    }

    // ── Очистка линий ─────────────────────────────────────────

    private (int count, List<(bool isRow, int index)> lines) ClearFullLines()
    {
        int count = 0;
        var lines = new List<(bool isRow, int index)>();

        for (int r = 0; r < Rows; r++)
            if (IsRowFull(r)) { lines.Add((true, r)); count++; }

        for (int c = 0; c < Cols; c++)
            if (IsColFull(c)) { lines.Add((false, c)); count++; }

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

    public void Reset() => InitGrid();
}