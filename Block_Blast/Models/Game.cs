using Block_Blast.GameModels;
using Block_Blast.Services;

namespace Block_Blast.Models;

public class Game
{
    // ── Поля ──────────────────────────────────────────────────
    private readonly BlockFactory _factory;
    private readonly GameMode _mode;

    // ── Свойства ──────────────────────────────────────────────
    public Board Board { get; private set; }
    public Player Player { get; private set; }
    public List<Block> NextBlocks { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsOver { get; private set; }

    // ── События ───────────────────────────────────────────────
    public event Action? GameStarted;
    public event Action? GameOver;
    public event Action<int>? ScoreChanged;

    /// <summary>
    /// Передаёт: кол-во линий, список линий (isRow + index), текущий комбо.
    /// </summary>
    public event Action<int, List<(bool isRow, int index)>, int>? LinesCleared;

    public event Action? BoardUpdated;

    // ── Конструктор ───────────────────────────────────────────
    public Game(Player player, GameMode mode = GameMode.Easy)
    {
        Player = player;
        _mode = mode;
        Board = new Board();
        _factory = new BlockFactory();
        NextBlocks = new List<Block>();

        Board.LinesCleared += OnLinesCleared;
    }

    // ── Управление игрой ──────────────────────────────────────

    public void Start()
    {
        Player.ResetScore();
        Board.Reset();
        IsOver = false;
        IsRunning = true;

        // Сложный режим — заполняем поле случайными клетками
        if (_mode == GameMode.Hard)
            Board.FillRandom(12);

        NextBlocks = _factory.GetNext3();

        GameStarted?.Invoke();
        ScoreChanged?.Invoke(Player.Score);
    }

    public void Stop() => IsRunning = false;

    // ── Игровая логика ────────────────────────────────────────

    public bool TryPlaceBlock(int blockIndex, int row, int col)
    {
        if (!IsRunning || IsOver) return false;
        if (blockIndex < 0 || blockIndex >= NextBlocks.Count) return false;

        var block = NextBlocks[blockIndex];
        if (!Board.CanPlace(block, row, col)) return false;

        // Очки за размещение
        int cellsPlaced = block.GetCells().Count;
        Player.AddPlacementScore(cellsPlaced);

        // Флаг: были ли очищены линии (обработается в OnLinesCleared)
        _lineClearedThisTurn = false;

        Board.PlaceBlock(block, row, col);

        // Если линий не было — сбрасываем комбо
        if (!_lineClearedThisTurn)
            Player.ResetCombo();

        NextBlocks.RemoveAt(blockIndex);
        if (NextBlocks.Count == 0)
            NextBlocks = _factory.GetNext3();

        BoardUpdated?.Invoke();
        ScoreChanged?.Invoke(Player.Score);

        if (CheckGameOver())
        {
            IsOver = true;
            IsRunning = false;
            GameOver?.Invoke();
        }

        return true;
    }

    public bool CheckGameOver() => Board.IsGameOver(NextBlocks);

    // ── Вспомогательное ───────────────────────────────────────
    private bool _lineClearedThisTurn = false;

    private void OnLinesCleared(int count, List<(bool isRow, int index)> lines)
    {
        _lineClearedThisTurn = true;
        Player.AddLineScore(count);
        LinesCleared?.Invoke(count, lines, Player.ComboCount);
        ScoreChanged?.Invoke(Player.Score);
    }
}