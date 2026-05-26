using Block_Blast.GameModels;
using Block_Blast.Models;
using Block_Blast.Resources.Localization;
using Block_Blast.Services;
using Microsoft.Maui.Layouts;

namespace Block_Blast.Pages;

public partial class GamePage : ContentPage
{
    private const int CellSize = 36;
    private const int BlockAreaSize = 100;
    private const int MiniCellSize = 22;
    private readonly ThemeService _themeService;
    private readonly ScoreService _scoreService;
    private readonly Game _game;
    private Label LblScoreTitle;
    private Label LblBestTitle;
    private Label LblScore;
    private Label LblBest;
    private Grid GameGrid;
    private readonly ContentView[] BlockContainers = new ContentView[3];
    private BoxView[,] _cellViews;
    private AbsoluteLayout _rootAbsolute;
    private Border _ghostView;
    private int _draggingIndex = -1;
    private bool _isDragging;
    private Point _ghostStartPos;

    private List<(int row, int col)> _previewCells = new();

    private List<(int row, int col)> _lineClearPreviewCells = new();

    private static readonly Color LineClearHighlight = Color.FromArgb("#FFFFAA");

    private List<(int row, int col)> _hintCells = new();

    private int _hintIndex = -1;
    public GamePage(
        Player player,
        GameMode mode,
        ThemeService themeService,
        ScoreService scoreService)
    {
        _themeService = themeService;
        _scoreService = scoreService;
        _game = new Game(player, mode);

        BuildUI();

        _game.ScoreChanged += OnScoreChanged;
        _game.BoardUpdated += OnBoardUpdated;
        _game.LinesCleared += OnLinesCleared;
        _game.GameOver += OnGameOver;
    }

    private void BuildUI()
    {
        BackgroundColor = Color.FromArgb("#0D0D1A");

        LblScoreTitle = new Label
        {
            FontFamily = "PressStart2P",
            FontSize = 20,
            TextColor = Color.FromArgb("#888899"),
            HorizontalOptions = LayoutOptions.Center,
            CharacterSpacing = 2
        };
        LblScore = new Label
        {
            Text = "0",
            FontFamily = "PressStart2P",
            FontSize = 20,
            TextColor = Color.FromArgb("#00F5FF"),
            HorizontalOptions = LayoutOptions.Center,
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Color.FromArgb("#00F5FF")),
                Offset = new Point(3, 3),
                Radius = 0,
                Opacity = 0.6f
            }
        };
        LblBestTitle = new Label
        {
            FontFamily = "PressStart2P",
            FontSize = 20,
            TextColor = Color.FromArgb("#888899"),
            HorizontalOptions = LayoutOptions.Center,
            CharacterSpacing = 2
        };
        LblBest = new Label
        {
            Text = "0",
            FontFamily = "PressStart2P",
            FontSize = 20,
            TextColor = Color.FromArgb("#FFE500"),
            HorizontalOptions = LayoutOptions.Center,
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Color.FromArgb("#FFE500")),
                Offset = new Point(3, 3),
                Radius = 0,
                Opacity = 0.6f
            }
        };

        var scoreCard = new Border
        {
            BackgroundColor = Color.FromArgb("#1A1A2E"),
            StrokeThickness = 2,
            Stroke = new SolidColorBrush(Color.FromArgb("#00F5FF")),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.Rectangle(),
            Padding = new Thickness(12, 10),
            Content = new Grid
            {
                ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            }
            }
        };

        var scoreGrid = (Grid)scoreCard.Content;
        scoreGrid.Add(new VerticalStackLayout { Spacing = 4, Children = { LblScoreTitle, LblScore } }, 0, 0);
        scoreGrid.Add(new VerticalStackLayout { Spacing = 4, Children = { LblBestTitle, LblBest } }, 1, 0);

        GameGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            RowSpacing = 0,
            ColumnSpacing = 0
        };

        var boardCard = new Border
        {
            BackgroundColor = Color.FromArgb("#1A1A2E"),
            StrokeThickness = 2,
            Stroke = new SolidColorBrush(Color.FromArgb("#39FF14")),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.Rectangle(),
            Padding = new Thickness(6),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Content = GameGrid
        };

        var blocksRow = new HorizontalStackLayout
        {
            Spacing = 20,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        for (int i = 0; i < 3; i++)
        {
            var container = new ContentView
            {
                WidthRequest = BlockAreaSize,
                HeightRequest = BlockAreaSize,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            BlockContainers[i] = container;
            blocksRow.Children.Add(container);
        }

        var blocksCard = new Border
        {
            BackgroundColor = Color.FromArgb("#1A1A2E"),
            StrokeThickness = 2,
            Stroke = new SolidColorBrush(Color.FromArgb("#FF3CAC")),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.Rectangle(),
            Padding = new Thickness(14, 10),
            Content = blocksRow
        };

        var pageContentGrid = new Grid
        {
            RowDefinitions =
        {
            new RowDefinition(GridLength.Auto),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Auto)
        },
            Padding = new Thickness(16, 48, 16, 24),
            RowSpacing = 16
        };
        pageContentGrid.Add(scoreCard, 0, 0);
        pageContentGrid.Add(boardCard, 0, 1);
        pageContentGrid.Add(blocksCard, 0, 2);

        _rootAbsolute = new AbsoluteLayout();
        AbsoluteLayout.SetLayoutBounds(pageContentGrid, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(pageContentGrid, AbsoluteLayoutFlags.SizeProportional);
        _rootAbsolute.Add(pageContentGrid);

        Content = _rootAbsolute;
    }

    private void ApplyTheme()
    {
        var t = _themeService.Current;
        bool isLight = t.Name == AppResources.light;
        bool isColorful = t.Name == AppResources.colorful;

        BackgroundColor = t.BackgroundColor;

        LblScoreTitle.TextColor = t.TextColor;
        LblBestTitle.TextColor = t.TextColor;

        
        LblScore.TextColor = t.AccentColor;

     
        LblBest.TextColor = isLight
            ? Color.FromArgb("#B8860B") 
            : isColorful
                ? Color.FromArgb("#FFE500")  
                : Colors.Gold;               
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LanguageService.LanguageChanged += ApplyLocalization;
        ApplyLocalization();
        ApplyTheme();
        BuildBoard();
        _game.Start();
        DrawBoard();
        DrawNextBlocks();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= ApplyLocalization;
        _game.ScoreChanged -= OnScoreChanged;
        _game.BoardUpdated -= OnBoardUpdated;
        _game.LinesCleared -= OnLinesCleared;
        _game.GameOver -= OnGameOver;
    }

    
    private void ApplyLocalization()
    {
        LblScoreTitle.Text = AppResources.score;
        LblBestTitle.Text = "🏆";
    }

    private void BuildBoard()
    {
        GameGrid.Children.Clear();
        GameGrid.RowDefinitions.Clear();
        GameGrid.ColumnDefinitions.Clear();

        _cellViews = new BoxView[Board.Rows, Board.Cols];

        for (int r = 0; r < Board.Rows; r++)
            GameGrid.RowDefinitions.Add(new RowDefinition { Height = CellSize });
        for (int c = 0; c < Board.Cols; c++)
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = CellSize });

        var theme = _themeService.Current;

        for (int r = 0; r < Board.Rows; r++)
        {
            for (int c = 0; c < Board.Cols; c++)
            {
                var cell = new BoxView
                {
                    BackgroundColor = theme.CellEmptyColor,
                    Margin = new Thickness(1),
                    CornerRadius = 0
                };
                _cellViews[r, c] = cell;
                GameGrid.Add(cell, c, r);
            }
        }
    }

    private void DrawBoard()
    {
        var theme = _themeService.Current;
        for (int r = 0; r < Board.Rows; r++)
            for (int c = 0; c < Board.Cols; c++)
            {
                var cell = _game.Board.Grid[r, c];
                _cellViews[r, c].BackgroundColor =
                    cell.IsOccupied ? cell.CellColor : theme.CellEmptyColor;
            }
    }

    private void ShowPreview(List<(int row, int col)> cells, Color blockColor)
    {
        ClearPreview();
        _previewCells = cells;

        
        var previewColor = Color.FromRgba(
            blockColor.Red, blockColor.Green, blockColor.Blue, 0.40f);

        foreach (var (r, c) in cells)
            _cellViews[r, c].BackgroundColor = previewColor;
    }

    private void ClearPreview()
    {
        if (_previewCells.Count == 0) return;
        var theme = _themeService.Current;
        foreach (var (r, c) in _previewCells)
        {
            var cell = _game.Board.Grid[r, c];
            _cellViews[r, c].BackgroundColor =
                cell.IsOccupied ? cell.CellColor : theme.CellEmptyColor;
        }
        _previewCells.Clear();
    }

    private void ShowLineClearPreview(int blockIndex, int startRow, int startCol)
    {
        ClearLineClearPreview();

        if (blockIndex >= _game.NextBlocks.Count) return;

        var block = _game.NextBlocks[blockIndex];
        var lines = _game.Board.GetWouldClearLines(block, startRow, startCol);

        if (lines.Count == 0) return;

        foreach (var (isRow, idx) in lines)
        {
            if (isRow)
            {
                for (int c = 0; c < Board.Cols; c++)
                {
                    _cellViews[idx, c].BackgroundColor = LineClearHighlight;
                    _lineClearPreviewCells.Add((idx, c));
                }
            }
            else
            {
                for (int r = 0; r < Board.Rows; r++)
                {
                    _cellViews[r, idx].BackgroundColor = LineClearHighlight;
                    _lineClearPreviewCells.Add((r, idx));
                }
            }
        }
    }

    private void ClearLineClearPreview()
    {
        if (_lineClearPreviewCells.Count == 0) return;
        var theme = _themeService.Current;
        foreach (var (r, c) in _lineClearPreviewCells)
        {
            if (_previewCells.Contains((r, c)))
            {
               
            }
            else
            {
                var cell = _game.Board.Grid[r, c];
                _cellViews[r, c].BackgroundColor =
                    cell.IsOccupied ? cell.CellColor : theme.CellEmptyColor;
            }
        }
        _lineClearPreviewCells.Clear();
    }

    private void ShowHint(int blockIndex)
    {
        ClearHint();

        if (blockIndex >= _game.NextBlocks.Count) return;

        var block = _game.NextBlocks[blockIndex];
        var theme = _themeService.Current;

        var hintColor = Color.FromRgba(
            block.BlockColor.Red,
            block.BlockColor.Green,
            block.BlockColor.Blue,
            0.18f);

        _hintIndex = blockIndex;

        for (int r = 0; r < Board.Rows; r++)
        {
            for (int c = 0; c < Board.Cols; c++)
            {
                var cells = _game.Board.GetPreviewCells(block, r, c);
                if (cells.Count == 0) continue;
                foreach (var (pr, pc) in cells)
                {
                    if (!_hintCells.Contains((pr, pc)))
                    {
                        _hintCells.Add((pr, pc));
                        if (!_game.Board.Grid[pr, pc].IsOccupied)
                            _cellViews[pr, pc].BackgroundColor = hintColor;
                    }
                }
            }
        }
    }

    private void ClearHint()
    {
        if (_hintCells.Count == 0) return;
        var theme = _themeService.Current;
        foreach (var (r, c) in _hintCells)
        {
            var cell = _game.Board.Grid[r, c];
            _cellViews[r, c].BackgroundColor =
                cell.IsOccupied ? cell.CellColor : theme.CellEmptyColor;
        }
        _hintCells.Clear();
        _hintIndex = -1;
    }
    private (int row, int col, bool valid) GetBoardCell(int blockIndex)
    {
        if (_ghostView == null) return (-1, -1, false);

        var block = _game.NextBlocks[blockIndex];
        var boardPos = GetAbsolutePosition(GameGrid);

        double ghostLeft = _ghostStartPos.X + _ghostView.TranslationX;
        double ghostTop = _ghostStartPos.Y + _ghostView.TranslationY;
        double blockPixelW = block.Cols * (MiniCellSize + 2);
        double blockPixelH = block.Rows * (MiniCellSize + 2);
        double offsetX = (BlockAreaSize - blockPixelW) / 2.0;
        double offsetY = (BlockAreaSize - blockPixelH) / 2.0;
        double blockLeft = ghostLeft + offsetX;
        double blockTop = ghostTop + offsetY;
        double blockCenterX = blockLeft + blockPixelW / 2.0;
        double blockCenterY = blockTop + blockPixelH / 2.0;
        double relX = blockCenterX - boardPos.X;
        double relY = blockCenterY - boardPos.Y;

        int col = (int)(relX / CellSize) - block.Cols / 2;
        int row = (int)(relY / CellSize) - block.Rows / 2;

        bool valid = row >= 0 && row < Board.Rows && col >= 0 && col < Board.Cols;
        return (row, col, valid);
    }
    private void UpdateBlockAvailability()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i >= _game.NextBlocks.Count) continue;
            var block = _game.NextBlocks[i];
            bool canPlace = _game.Board.CanPlaceAnywhere(block);
            BlockContainers[i].Opacity = canPlace ? 1.0 : 0.35;
        }
    }

    private void DrawNextBlocks()
    {
        ClearHint();
        for (int i = 0; i < 3; i++)
            DrawMiniBlock(i);
        UpdateBlockAvailability();
    }

    private void DrawMiniBlock(int index)
    {
        var container = BlockContainers[index];
        container.Content = null;
        container.GestureRecognizers.Clear();

        if (index >= _game.NextBlocks.Count) return;

        var block = _game.NextBlocks[index];

        var grid = new Grid
        {
            RowSpacing = 0,
            ColumnSpacing = 0,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            InputTransparent = true
        };

        for (int r = 0; r < block.Rows; r++)
            grid.RowDefinitions.Add(new RowDefinition { Height = MiniCellSize });
        for (int c = 0; c < block.Cols; c++)
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = MiniCellSize });

        foreach (var (dr, dc) in block.GetCells())
        {
            grid.Add(new Border
            {
                BackgroundColor = block.BlockColor,
                StrokeThickness = 0,
                Padding = 0,
                Margin = 1
            }, dc, dr);
        }

        container.Content = grid;
        SetupDrag(index);
    }

    private void SetupDrag(int idx)
    {
        var container = BlockContainers[idx];

        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) =>
        {

            if (_hintIndex == idx)
                ClearHint();
            else
                ShowHint(idx);
        };
        container.GestureRecognizers.Add(tap);

        var pan = new PanGestureRecognizer();

        pan.PanUpdated += async (s, e) =>
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    {
                        if (_isDragging) break;
                        _isDragging = true;
                        _draggingIndex = idx;

                        ClearHint();

                        _ = container.FadeToAsync(0.25, 100);

                        _ghostStartPos = GetAbsolutePosition(container);
                        _ghostView = BuildGhostView(idx);

                        AbsoluteLayout.SetLayoutBounds(
                            _ghostView,
                            new Rect(_ghostStartPos.X, _ghostStartPos.Y, BlockAreaSize, BlockAreaSize));
                        AbsoluteLayout.SetLayoutFlags(_ghostView, AbsoluteLayoutFlags.None);

                        _rootAbsolute.Add(_ghostView);
                        break;
                    }

                case GestureStatus.Running:
                    {
                        if (_ghostView == null || !_isDragging) break;

                        _ghostView.TranslationX = e.TotalX;
                        _ghostView.TranslationY = e.TotalY;

                        var (row, col, valid) = GetBoardCell(idx);
                        if (valid)
                        {
                            var block = _game.NextBlocks[idx];

                            var preview = _game.Board.GetPreviewCells(block, row, col);
                            int finalRow = row, finalCol = col;

                            if (preview.Count == 0)
                            {
                                for (int dr = -1; dr <= 1 && preview.Count == 0; dr++)
                                    for (int dc = -1; dc <= 1 && preview.Count == 0; dc++)
                                    {
                                        var p = _game.Board.GetPreviewCells(block, row + dr, col + dc);
                                        if (p.Count > 0)
                                        {
                                            preview = p;
                                            finalRow = row + dr;
                                            finalCol = col + dc;
                                        }
                                    }
                            }

                            if (preview.Count > 0)
                            {
                                ShowPreview(preview, block.BlockColor);
                                ShowLineClearPreview(idx, finalRow, finalCol);
                            }
                            else
                            {
                                ClearLineClearPreview();
                                ClearPreview();
                            }
                        }
                        else
                        {
                            ClearLineClearPreview();
                            ClearPreview();
                        }
                        break;
                    }

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    {
                        if (!_isDragging) break;
                        _isDragging = false;

                        ClearHint();
                        ClearLineClearPreview();
                        ClearPreview();
                        _ = container.FadeToAsync(1.0, 100);

                        bool placed = false;

                        if (e.StatusType == GestureStatus.Completed && _ghostView != null)
                        {
                            var (row, col, valid) = GetBoardCell(idx);

                            if (valid)
                            {
                                placed = _game.TryPlaceBlock(idx, row, col);

                                if (!placed)
                                {
                                    for (int dr = -1; dr <= 1 && !placed; dr++)
                                        for (int dc = -1; dc <= 1 && !placed; dc++)
                                            placed = _game.TryPlaceBlock(idx, row + dr, col + dc);
                                }
                            }
                        }

                        if (_ghostView != null)
                        {
                            var ghost = _ghostView;
                            _ghostView = null;

                            if (!placed)
                            {
                                await Task.WhenAll(
                                    ghost.TranslateToAsync(0, 0, 180, Easing.SpringOut),
                                    ghost.FadeToAsync(0, 180));
                            }
                            else
                            {
                                await ghost.FadeToAsync(0, 100);
                            }

                            _rootAbsolute.Remove(ghost);
                        }

                        _draggingIndex = -1;
                        break;
                    }
            }
        };

        container.GestureRecognizers.Add(pan);
    }

    private Border BuildGhostView(int idx)
    {
        var block = _game.NextBlocks[idx];
        var grid = new Grid
        {
            RowSpacing = 0,
            ColumnSpacing = 0,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            InputTransparent = true,
            ScaleX = 1.15,
            ScaleY = 1.15
        };

        for (int r = 0; r < block.Rows; r++)
            grid.RowDefinitions.Add(new RowDefinition { Height = MiniCellSize });
        for (int c = 0; c < block.Cols; c++)
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = MiniCellSize });

        foreach (var (dr, dc) in block.GetCells())
        {
            grid.Add(new Border
            {
                BackgroundColor = block.BlockColor,
                StrokeThickness = 0,
                Padding = 0,
                Margin = 1,
                Opacity = 0.85
            }, dc, dr);
        }

        return new Border
        {
            WidthRequest = BlockAreaSize,
            HeightRequest = BlockAreaSize,
            BackgroundColor = Colors.Transparent,
            StrokeThickness = 0,
            Padding = 0,
            InputTransparent = true,
            Content = grid
        };
    }

    private Point GetAbsolutePosition(View view)
    {
        double x = 0, y = 0;
        Element current = view;
        while (current != null && current != _rootAbsolute)
        {
            if (current is VisualElement ve) { x += ve.Bounds.X; y += ve.Bounds.Y; }
            current = current.Parent;
        }
        return new Point(x, y);
    }

    private async Task AnimateClearLines(List<(bool isRow, int index)> lines)
    {
        var tasks = new List<Task>();

        foreach (var (isRow, idx) in lines)
        {
            if (isRow)
            {

                for (int c = 0; c < Board.Cols; c++)
                {
                    var cell = _cellViews[idx, c];
                    int delay = c * 25;
                    tasks.Add(AnimateCellDisappear(cell, delay));
                }
            }
            else
            {
                for (int r = 0; r < Board.Rows; r++)
                {
                    var cell = _cellViews[idx, r];
                    int delay = r * 25;
                    tasks.Add(AnimateCellDisappear(cell, delay));
                }
            }
        }

        await Task.WhenAll(tasks);

        DrawBoard();

        for (int r = 0; r < Board.Rows; r++)
            for (int c = 0; c < Board.Cols; c++)
                _cellViews[r, c].Opacity = 1;
    }

    private static async Task AnimateCellDisappear(BoxView cell, int delayMs)
    {
        await Task.Delay(delayMs);
        await cell.ScaleToAsync(0.1, 120, Easing.CubicIn);
        cell.Opacity = 0;
        cell.Scale = 1;
    }
   private async Task ShowComboLabel(int combo)
    {
        if (combo < 2) return; 

        string text = combo switch
        {
            2 => $"{AppResources.combo1} ×1.5! 🔥",
            3 => $"{AppResources.combo1} ×2! 🔥🔥",
            4 => $"{AppResources.combo1} ×2.5! 💥",
            _ => $"{AppResources.combo1} ×3! 🌟"
        };

        var label = new Label
        {
            Text = text,
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.OrangeRed,
            HorizontalOptions = LayoutOptions.Center,
            Opacity = 0,
            TranslationY = 0,
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Colors.Yellow),
                Offset = new Point(2, 2),
                Radius = 4,
                Opacity = 0.8f
            }
        };

        var boardPos = GetAbsolutePosition(GameGrid);
        double cx = boardPos.X + (Board.Cols * CellSize) / 2.0 - 80;
        double cy = boardPos.Y + (Board.Rows * CellSize) / 2.0 - 20;

        AbsoluteLayout.SetLayoutBounds(label, new Rect(cx, cy, 200, 50));
        AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.None);
        _rootAbsolute.Add(label);

        await Task.WhenAll(
            label.FadeToAsync(1, 150),
            label.TranslateToAsync(0, -20, 150, Easing.CubicOut));

        await Task.Delay(500);

        await Task.WhenAll(
            label.FadeToAsync(0, 300),
            label.TranslateToAsync(0, -50, 300, Easing.CubicIn));

        _rootAbsolute.Remove(label);
    }
    private void OnScoreChanged(int score)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LblScore.Text = score.ToString("N0");
            LblBest.Text = _game.Player.BestScore.ToString("N0");

            _ = LblScore.ScaleToAsync(1.25, 80).ContinueWith(_ =>
                MainThread.BeginInvokeOnMainThread(() =>
                    LblScore.ScaleToAsync(1.0, 100)));
        });
    }

    private void OnBoardUpdated()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            DrawBoard();
            DrawNextBlocks();
        });
    }

    private void OnLinesCleared(int count, List<(bool isRow, int index)> lines, int combo, bool isBoardClear)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var tasks = new List<Task>
            {
                AnimateClearLines(lines),
                ShowComboLabel(combo)
            };

            if (isBoardClear)
                tasks.Add(ShowBoardClearLabel());

            await Task.WhenAll(tasks);
        });
    }
    private async Task ShowBoardClearLabel()
    {
        var label = new Label
        {
            Text = "✨ "+AppResources.board_clear+" +360",
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Gold,
            HorizontalOptions = LayoutOptions.Center,
            Opacity = 0,
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Colors.OrangeRed),
                Offset = new Point(2, 2),
                Radius = 6,
                Opacity = 0.9f
            }
        };

        var boardPos = GetAbsolutePosition(GameGrid);
        double cx = boardPos.X + (Board.Cols * CellSize) / 2.0 - 110;
        double cy = boardPos.Y + 30;

        AbsoluteLayout.SetLayoutBounds(label, new Rect(cx, cy, 240, 50));
        AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.None);
        _rootAbsolute.Add(label);

        await Task.WhenAll(
            label.FadeToAsync(1, 200),
            label.ScaleToAsync(1.15, 200, Easing.CubicOut));

        await Task.Delay(700);

        await Task.WhenAll(
            label.FadeToAsync(0, 350),
            label.TranslateToAsync(0, -40, 350, Easing.CubicIn));

        _rootAbsolute.Remove(label);
    }

    private void OnGameOver()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            _scoreService.Save(_game.Player);

            // Детальная статистика в сообщении
            string stats =
                $"{AppResources.score}: {_game.Player.Score:N0}\n" +
                $"{AppResources.lines}: {_game.Player.TotalLinesCleared}\n" +
                $"{AppResources.best_combo}: ×{1.0 + (_game.Player.MaxCombo - 1) * 0.5:0.0}";

            bool again = await DisplayAlertAsync(
                AppResources.gameover,
                stats,
                AppResources.playagain,
                AppResources.menu);

            if (again)
            {
                _game.Start();
                DrawBoard();
                DrawNextBlocks();
            }
            else
            {
                await Navigation.PopAsync();
            }
        });
    }
}