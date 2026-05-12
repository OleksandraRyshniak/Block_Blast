using Block_Blast.GameModels;
using Block_Blast.Models;
using Block_Blast.Resources.Localization;
using Block_Blast.Services;

namespace Block_Blast.Pages;

public partial class GamePage : ContentPage
{
    // ══════════════════════════════════════════════════════════
    // Константы
    // ══════════════════════════════════════════════════════════

    private const int CellSize = 36;
    private const int MiniCellSize = 20;

    // ══════════════════════════════════════════════════════════
    // Поля
    // ══════════════════════════════════════════════════════════

    private readonly ThemeService _themeService;
 
    private readonly ScoreService _scoreService;
    private readonly Game _game;

    private int _draggedBlockIndex = -1;
    private BoxView[,] _cellViews;

    // ══════════════════════════════════════════════════════════
    // UI элементы
    // ══════════════════════════════════════════════════════════

    private Label LblScoreTitle;
    private Label LblBestTitle;
    private Label LblScore;
    private Label LblBest;

    private Grid GameGrid;

    private Frame Block0Frame;
    private Frame Block1Frame;
    private Frame Block2Frame;

    private Grid Block0Grid;
    private Grid Block1Grid;
    private Grid Block2Grid;

    // ══════════════════════════════════════════════════════════
    // Конструктор
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
    // UI
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

        var scoreGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            Padding = new Thickness(10)
        };

        scoreGrid.Add(
            new VerticalStackLayout
            {
                Children = { LblScoreTitle, LblScore }
            }, 0, 0);

        scoreGrid.Add(
            new VerticalStackLayout
            {
                Children = { LblBestTitle, LblBest }
            }, 1, 0);

        GameGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            RowSpacing = 0,
            ColumnSpacing = 0
        };

        Block0Grid = CreateMiniGrid();
        Block1Grid = CreateMiniGrid();
        Block2Grid = CreateMiniGrid();

        Block0Frame = CreateBlockFrame(Block0Grid);
        Block1Frame = CreateBlockFrame(Block1Grid);
        Block2Frame = CreateBlockFrame(Block2Grid);

        var blocksLayout = new HorizontalStackLayout
        {
            Spacing = 20,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                Block0Frame,
                Block1Frame,
                Block2Frame
            }
        };

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(20, 50, 20, 20),
                Spacing = 25,
                Children =
                {
                    scoreGrid,
                    GameGrid,
                    blocksLayout
                }
            }
        };
    }

    private Grid CreateMiniGrid()
    {
        return new Grid
        {
            RowSpacing = 0,
            ColumnSpacing = 0,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
    }

    private Frame CreateBlockFrame(Grid content)
    {
        return new Frame
        {
            Padding = 8,
            CornerRadius = 12,
            BackgroundColor = Colors.DimGray,
            BorderColor = Colors.White,
            HasShadow = true,
            Content = content,
            WidthRequest = 90,
            HeightRequest = 90
        };
    }

    // ══════════════════════════════════════════════════════════
    // Жизненный цикл
    // ══════════════════════════════════════════════════════════

    protected override void OnAppearing()
    {
        base.OnAppearing();

        ApplyTheme();
        ApplyLocalization();

        BuildBoard();

        _game.Start();

        DrawBoard();
        DrawNextBlocks();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _game.ScoreChanged -= OnScoreChanged;
        _game.BoardUpdated -= OnBoardUpdated;
        _game.LinesCleared -= OnLinesCleared;
        _game.GameOver -= OnGameOver;
    }

    // ══════════════════════════════════════════════════════════
    // Тема и локализация
    // ══════════════════════════════════════════════════════════

    private void ApplyTheme()
    {
        _themeService.Current.Apply(this);
    }

    private void ApplyLocalization()
    {
        LblScoreTitle.Text = AppResources.score;
        LblBestTitle.Text = AppResources.best;
    }

    // ══════════════════════════════════════════════════════════
    // Поле
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
                    CornerRadius = 4
                };

                int row = r;
                int col = c;

                var dropGesture = new DropGestureRecognizer();

                dropGesture.DragOver += (s, e) =>
                {
                    e.AcceptedOperation = DataPackageOperation.Copy;
                };

                dropGesture.Drop += (s, e) =>
                {
                    OnDrop(row, col);
                };

                cell.GestureRecognizers.Add(dropGesture);

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
                    cell.IsOccupied
                        ? cell.CellColor
                        : theme.CellEmptyColor;
            }
        }
    }

    // ══════════════════════════════════════════════════════════
    // Следующие блоки
    // ══════════════════════════════════════════════════════════

    private void DrawNextBlocks()
    {
        DrawMiniBlock(Block0Grid, 0);
        DrawMiniBlock(Block1Grid, 1);
        DrawMiniBlock(Block2Grid, 2);
    }

    private void DrawMiniBlock(Grid grid, int index)
    {
        grid.Children.Clear();
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();

        if (index >= _game.NextBlocks.Count)
            return;

        var block = _game.NextBlocks[index];

        for (int r = 0; r < block.Rows; r++)
            grid.RowDefinitions.Add(new RowDefinition { Height = MiniCellSize });

        for (int c = 0; c < block.Cols; c++)
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = MiniCellSize });

        foreach (var (dr, dc) in block.GetCells())
        {
            var cell = new BoxView
            {
                BackgroundColor = block.BlockColor,
                Margin = new Thickness(1),
                CornerRadius = 3
            };

            grid.Add(cell, dc, dr);
        }

        SetupDragOnFrame(index);
    }

    // ══════════════════════════════════════════════════════════
    // Drag & Drop
    // ══════════════════════════════════════════════════════════

    private void SetupDragOnFrame(int blockIndex)
    {
        Frame frame = blockIndex switch
        {
            0 => Block0Frame,
            1 => Block1Frame,
            _ => Block2Frame
        };

        frame.GestureRecognizers.Clear();

        var drag = new DragGestureRecognizer();

        drag.DragStarting += (s, e) =>
        {
            _draggedBlockIndex = blockIndex;
            e.Data.Properties["blockIndex"] = blockIndex;
        };

        frame.GestureRecognizers.Add(drag);
    }

    private void OnDrop(int row, int col)
    {
        if (_draggedBlockIndex < 0)
            return;

        bool placed = _game.TryPlaceBlock(_draggedBlockIndex, row, col);

        if (!placed)
        {
            Frame frame = _draggedBlockIndex switch
            {
                0 => Block0Frame,
                1 => Block1Frame,
                _ => Block2Frame
            };

            ShakeAnimation(frame);
        }

        _draggedBlockIndex = -1;
    }

    // ══════════════════════════════════════════════════════════
    // Анимации
    // ══════════════════════════════════════════════════════════

    private async void ShakeAnimation(View view)
    {
        for (int i = 0; i < 3; i++)
        {
            await view.TranslateTo(-8, 0, 50);
            await view.TranslateTo(8, 0, 50);
        }

        await view.TranslateTo(0, 0, 50);
    }

    private async void FlashAnimation()
    {
        for (int i = 0; i < 2; i++)
        {
            await GameGrid.FadeTo(0.3, 80);
            await GameGrid.FadeTo(1.0, 80);
        }
    }

    // ══════════════════════════════════════════════════════════
    // События игры
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
        MainThread.BeginInvokeOnMainThread(() =>
        {
            FlashAnimation();
        });
    }

    private void OnGameOver()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            _scoreService.Save(_game.Player);

            string title = AppResources.gameover;
            string message = $"{AppResources.yoursroce}: {_game.Player.Score:N0}";
            string again = AppResources.playagain;
            string menu = AppResources.menu;

            bool playAgain = await DisplayAlert(title, message, again, menu);

            if (playAgain)
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

