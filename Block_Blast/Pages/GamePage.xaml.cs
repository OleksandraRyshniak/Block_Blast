using Block_Blast.GameModels;
using Block_Blast.Models;
using Block_Blast.Resources.Localization;
using Block_Blast.Services;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace Block_Blast.Pages;

public partial class GamePage : ContentPage
{
    // ══════════════════════════════════════════════════════════
    // CONSTANTS
    // ══════════════════════════════════════════════════════════

    private const int CellSize = 36;
    private const int BlockAreaSize = 100;
    private const int MiniCellSize = 22;

    // ══════════════════════════════════════════════════════════
    // SERVICES
    // ══════════════════════════════════════════════════════════

    private readonly ThemeService _themeService;
    private readonly ScoreService _scoreService;
    private readonly Game _game;

    // ══════════════════════════════════════════════════════════
    // UI
    // ══════════════════════════════════════════════════════════

    private Label LblScoreTitle;
    private Label LblBestTitle;
    private Label LblScore;
    private Label LblBest;

    private Grid GameGrid;

    private readonly ContentView[] BlockContainers = new ContentView[3];

    private BoxView[,] _cellViews;

    // ══════════════════════════════════════════════════════════
    // DRAG STATE
    // ══════════════════════════════════════════════════════════

    private AbsoluteLayout _rootAbsolute;
    private Border _ghostView;
    private int _draggingIndex = -1;
    private bool _isDragging;
    private Point _ghostStartPos;

    // ══════════════════════════════════════════════════════════
    // CONSTRUCTOR
    // ══════════════════════════════════════════════════════════

    public GamePage(
        Player player,
        ThemeService themeService,
        ScoreService scoreService)
    {
        _themeService = themeService;
        _scoreService = scoreService;

        _game = new Game(player);

        BuildUI();

        _game.ScoreChanged += OnScoreChanged;
        _game.BoardUpdated += OnBoardUpdated;
        _game.LinesCleared += OnLinesCleared;
        _game.GameOver += OnGameOver;
    }

    // ══════════════════════════════════════════════════════════
    // UI BUILD
    // ══════════════════════════════════════════════════════════

    private void BuildUI()
    {
        BackgroundColor = Colors.Black;

        LblScoreTitle = new Label
        {
            Text = AppResources.score,
            FontSize = 18,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        LblScore = new Label
        {
            Text = "0",
            FontSize = 24,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Lime,
            HorizontalOptions = LayoutOptions.Center
        };

        LblBestTitle = new Label
        {
            Text = AppResources.best,
            FontSize = 18,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        LblBest = new Label
        {
            Text = "0",
            FontSize = 24,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Gold,
            HorizontalOptions = LayoutOptions.Center
        };

        var scoreRow = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            Padding = new Thickness(10)
        };

        scoreRow.Add(new VerticalStackLayout { Children = { LblScoreTitle, LblScore } }, 0, 0);
        scoreRow.Add(new VerticalStackLayout { Children = { LblBestTitle, LblBest } }, 1, 0);

        GameGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            RowSpacing = 0,
            ColumnSpacing = 0
        };

        var blocksRow = new HorizontalStackLayout
        {
            Spacing = 25,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        for (int i = 0; i < 3; i++)
        {
            var container = new ContentView
            {
                WidthRequest = BlockAreaSize,
                HeightRequest = BlockAreaSize,
                BackgroundColor = Colors.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            BlockContainers[i] = container;
            blocksRow.Children.Add(container);
        }

        var pageContentGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            },
            Padding = new Thickness(20, 50, 20, 30),
            RowSpacing = 20
        };

        pageContentGrid.Add(scoreRow, 0, 0);
        pageContentGrid.Add(GameGrid, 0, 1);
        pageContentGrid.Add(blocksRow, 0, 2);

        // AbsoluteLayout как корень — необходим для ghost view
        _rootAbsolute = new AbsoluteLayout();

        AbsoluteLayout.SetLayoutBounds(pageContentGrid, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(pageContentGrid, AbsoluteLayoutFlags.SizeProportional);

        _rootAbsolute.Add(pageContentGrid);

        Content = _rootAbsolute;
    }

    // ══════════════════════════════════════════════════════════
    // LIFECYCLE
    // ══════════════════════════════════════════════════════════

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

    private void ApplyTheme() => _themeService.Current.Apply(this);

    private void ApplyLocalization()
    {
        LblScoreTitle.Text = AppResources.score;
        LblBestTitle.Text = AppResources.best;
    }

    // ══════════════════════════════════════════════════════════
    // BOARD
    // ══════════════════════════════════════════════════════════

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
        {
            for (int c = 0; c < Board.Cols; c++)
            {
                var cell = _game.Board.Grid[r, c];

                _cellViews[r, c].BackgroundColor =
                    cell.IsOccupied ? cell.CellColor : theme.CellEmptyColor;
            }
        }
    }

    // ══════════════════════════════════════════════════════════
    // BLOCKS
    // ══════════════════════════════════════════════════════════

    private void DrawNextBlocks()
    {
        for (int i = 0; i < 3; i++)
            DrawMiniBlock(i);
    }

    private void DrawMiniBlock(int index)
    {
        var container = BlockContainers[index];
        container.Content = null;
        container.GestureRecognizers.Clear();

        if (index >= _game.NextBlocks.Count)
            return;

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

    // ══════════════════════════════════════════════════════════
    // DRAG — ghost-подход как в CraftPage
    // ══════════════════════════════════════════════════════════

    private void SetupDrag(int idx)
    {
        var container = BlockContainers[idx];

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

                        _ = container.FadeTo(0.25, 100);

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
                        break;
                    }

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    {
                        if (!_isDragging) break;

                        _isDragging = false;

                        _ = container.FadeTo(1.0, 100);

                        bool placed = false;

                        if (e.StatusType == GestureStatus.Completed && _ghostView != null)
                        {
                            var block = _game.NextBlocks[idx];
                            var boardPos = GetAbsolutePosition(GameGrid);

                            // Верхний левый угол ghost в абсолютных координатах
                            double ghostLeft = _ghostStartPos.X + _ghostView.TranslationX;
                            double ghostTop = _ghostStartPos.Y + _ghostView.TranslationY;

                            // Размер мини-блока в пикселях
                            double blockPixelW = block.Cols * (MiniCellSize + 2);
                            double blockPixelH = block.Rows * (MiniCellSize + 2);

                            // Центр самого блока внутри контейнера 100×100
                            double offsetX = (BlockAreaSize - blockPixelW) / 2.0;
                            double offsetY = (BlockAreaSize - blockPixelH) / 2.0;

                            // Абсолютная позиция верхнего левого угла блока (не контейнера)
                            double blockLeft = ghostLeft + offsetX;
                            double blockTop = ghostTop + offsetY;

                            // Центр блока
                            double blockCenterX = blockLeft + blockPixelW / 2.0;
                            double blockCenterY = blockTop + blockPixelH / 2.0;

                            // Смещение центра блока относительно доски
                            double relX = blockCenterX - boardPos.X;
                            double relY = blockCenterY - boardPos.Y;

                            // Целевая клетка — верхний левый угол размещения блока
                            int col = (int)(relX / CellSize) - block.Cols / 2;
                            int row = (int)(relY / CellSize) - block.Rows / 2;

                            // Логируем для отладки
                            System.Diagnostics.Debug.WriteLine(
                                $"[Drag] ghostLeft={ghostLeft:F0} ghostTop={ghostTop:F0} " +
                                $"boardPos=({boardPos.X:F0},{boardPos.Y:F0}) " +
                                $"blockCenter=({blockCenterX:F0},{blockCenterY:F0}) " +
                                $"rel=({relX:F0},{relY:F0}) row={row} col={col}");

                            if (row >= 0 && row < Board.Rows &&
                                col >= 0 && col < Board.Cols)
                            {
                                placed = _game.TryPlaceBlock(idx, row, col);
                            }

                            // Если не попал точно — пробуем соседние клетки (±1)
                            if (!placed)
                            {
                                for (int dr = -1; dr <= 1 && !placed; dr++)
                                    for (int dc = -1; dc <= 1 && !placed; dc++)
                                    {
                                        int r2 = row + dr;
                                        int c2 = col + dc;
                                        if (r2 >= 0 && r2 < Board.Rows &&
                                            c2 >= 0 && c2 < Board.Cols)
                                        {
                                            placed = _game.TryPlaceBlock(idx, r2, c2);
                                        }
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
                                    ghost.TranslateTo(0, 0, 180, Easing.SpringOut),
                                    ghost.FadeTo(0, 180));
                            }
                            else
                            {
                                await ghost.FadeTo(0, 100);
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

    /// <summary>
    /// Строит ghost Border с мини-сеткой блока внутри.
    /// </summary>
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

    // ══════════════════════════════════════════════════════════
    // HELPERS
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// Обходим дерево родителей до _rootAbsolute,
    /// суммируя Bounds.X/Y — так же, как в CraftPage.
    /// </summary>
    private Point GetAbsolutePosition(View view)
    {
        double x = 0, y = 0;
        Element current = view;

        while (current != null && current != _rootAbsolute)
        {
            if (current is VisualElement ve)
            {
                x += ve.Bounds.X;
                y += ve.Bounds.Y;
            }

            current = current.Parent;
        }

        return new Point(x, y);
    }

    // ══════════════════════════════════════════════════════════
    // EFFECTS
    // ══════════════════════════════════════════════════════════

    private async Task Flash()
    {
        for (int i = 0; i < 2; i++)
        {
            await GameGrid.FadeTo(0.3, 80);
            await GameGrid.FadeTo(1.0, 80);
        }
    }

    // ══════════════════════════════════════════════════════════
    // EVENTS
    // ══════════════════════════════════════════════════════════

    private void OnScoreChanged(int score)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LblScore.Text = score.ToString("N0");
            LblBest.Text = _game.Player.BestScore.ToString("N0");
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

    private void OnLinesCleared(int count)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Flash();
        });
    }

    private void OnGameOver()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            _scoreService.Save(_game.Player);

            bool again = await DisplayAlert(
                AppResources.gameover,
                $"{AppResources.score}: {_game.Player.Score:N0}",
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