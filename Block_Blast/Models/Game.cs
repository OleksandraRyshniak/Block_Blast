using Block_Blast.GameModels;
using Block_Blast.Services;

namespace Block_Blast.Models;

public class Game
{
    
    private readonly BlockFactory _factory;
    private readonly GameMode _mode;
    private bool _lineClearedThisTurn;

    
    public Board Board { get; private set; }
    public Player Player { get; private set; }
    public List<Block> NextBlocks { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsOver { get; private set; }

   
    public event Action? GameStarted;
    public event Action? GameOver;
    public event Action<int>? ScoreChanged;


    public event Action<int, List<(bool isRow, int index)>, int, bool>? LinesCleared;

    public event Action? BoardUpdated;

    public Game(Player player, GameMode mode = GameMode.Easy)
    {
        Player = player;
        _mode = mode;
        Board = new Board();
        _factory = new BlockFactory();
        NextBlocks = new List<Block>();

        Board.LinesCleared += OnLinesCleared;
    }


    public void Start()
    {
        Player.ResetScore();
        Board.Reset();
        IsOver = false;
        IsRunning = true;

        if (_mode == GameMode.Hard)
            Board.FillRandom(12);

        NextBlocks = _factory.GetNext3();
        GameStarted?.Invoke();
        ScoreChanged?.Invoke(Player.Score);
    }

    public void Stop() => IsRunning = false;


    public bool TryPlaceBlock(int blockIndex, int row, int col)
    {
        if (!IsRunning || IsOver) return false;
        if (blockIndex < 0 || blockIndex >= NextBlocks.Count) return false;

        var block = NextBlocks[blockIndex];
        if (!Board.CanPlace(block, row, col)) return false;

        
        int cellsPlaced = block.GetCells().Count;
        Player.AddPlacementScore(cellsPlaced);

        _lineClearedThisTurn = false;

        Board.PlaceBlock(block, row, col);

        
        if (!_lineClearedThisTurn)
            Player.ResetCombo();

        NextBlocks.RemoveAt(blockIndex);
        if (NextBlocks.Count == 0)
            NextBlocks = _factory.GetNext3();

        BoardUpdated?.Invoke();
        ScoreChanged?.Invoke(Player.Score);

        if (Board.IsGameOver(NextBlocks))
        {
            IsOver = true;
            IsRunning = false;
            GameOver?.Invoke();
        }

        return true;
    }

    
    private void OnLinesCleared(int count, List<(bool isRow, int index)> lines, bool isBoardClear)
    {
        _lineClearedThisTurn = true;
        Player.AddLineScore(count, isBoardClear);
        LinesCleared?.Invoke(count, lines, Player.ComboCount, isBoardClear);
        ScoreChanged?.Invoke(Player.Score);
    }
}